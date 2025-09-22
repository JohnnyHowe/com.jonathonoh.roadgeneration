using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    [Serializable]
    public class RoadSectionPool
    {
        [SerializeField][Tooltip("Setting to zero will make this lazy initialize pieces.")] private int objectsPerSection = 5;
        [SerializeField][Tooltip("Must be at least check depth.")] private int minNumberOfTotalUnusedRoadSectionsToGenerateMore = 10;
        [SerializeField] private int minNumberOfUnusedRoadSectionsToCreateMore = 10;

        // id, pieces
        public Dictionary<string, List<RoadSection>> availableSections;
        public Dictionary<string, List<RoadSection>> usedSections;
        public Dictionary<string, RoadSection> prototypes;
        private Transform container;

        public void Reset(List<RoadSection> uninstantiatedRoadSections, Transform gameObjectContainer)
        {
            container = gameObjectContainer;
            CreatePrototypes(uninstantiatedRoadSections);
            CreateObjects();
            EnsureEnoughUnusedPiecesExist();
        }

        private void CreatePrototypes(List<RoadSection> uninstantiatedSections)
        {
            prototypes = new Dictionary<string, RoadSection>();
            foreach (RoadSection uninstantiatedSection in uninstantiatedSections)
            {
                RoadSection normalPiece = CreatePrototype(uninstantiatedSection);
                prototypes[normalPiece.GetFullId()] = normalPiece;

                RoadSection flippedPiece = CreatePrototype(uninstantiatedSection);
                flippedPiece.SetFlipped(true);
                flippedPiece.gameObject.name += " (flipped)";
                prototypes[flippedPiece.GetFullId()] = flippedPiece;
            }
        }

        private RoadSection CreatePrototype(RoadSection uninstantiatedSection)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(uninstantiatedSection.gameObject);

            gameObject.transform.parent = container;
            gameObject.SetActive(false);
            gameObject.transform.SetGlobalScale(container.lossyScale);

            RoadSection instantiatedSection = gameObject.GetComponent<RoadSection>();
            instantiatedSection.pieceTypeId = uninstantiatedSection.name;
            instantiatedSection.name = instantiatedSection.pieceTypeId + " (prototype)";

            return instantiatedSection;
        }

        private void CreateObjects()
        {
            // instantiate pool dictionaries
            availableSections = new Dictionary<string, List<RoadSection>>();
            usedSections = new Dictionary<string, List<RoadSection>>();

            foreach (RoadSection prototype in prototypes.Values)
            {
                availableSections[prototype.GetFullId()] = new List<RoadSection>();
                usedSections[prototype.GetFullId()] = new List<RoadSection>();
            }

            foreach (RoadSection prototype in prototypes.Values)
            {
                for (int i = 0; i < objectsPerSection; i++)
                {
                    AddInstantiatedObject(CreateObject(prototype));
                }
            }
        }

        private RoadSection CreateObject(RoadSection prototype)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(prototype.gameObject);

            gameObject.transform.parent = container;
            gameObject.SetActive(false);
            gameObject.transform.SetGlobalScale(container.lossyScale);

            RoadSection instantiatedSection = gameObject.GetComponent<RoadSection>();
            instantiatedSection.SetFlipped(prototype.IsFlipped);
            gameObject.name = instantiatedSection.GetFullId();
            return instantiatedSection;
        }

        public void AddInstantiatedObject(RoadSection instantiatedSection)
        {
            availableSections[instantiatedSection.GetFullId()].Add(instantiatedSection);
        }

        public RoadSection ClaimUninstantiatedSection(RoadSection uninstantiatedSection)
        {
            RoadSection instantiatedSection = GetAvailableSection(uninstantiatedSection);
            ClaimInstantiatedSection(instantiatedSection);
            return instantiatedSection;
        }

        private RoadSection GetAvailableSection(RoadSection uninstantiatedSection)
        {
            List<RoadSection> roadSections = availableSections[uninstantiatedSection.GetFullId()];

            // do we have one available?
            if (roadSections.Count == 0)
            {
                // Should never get here
                throw new Exception("No available section");
            }

            return roadSections[0];
        }

        public void ClaimInstantiatedSection(RoadSection instantiatedSection)
        {
            availableSections[instantiatedSection.GetFullId()].Remove(instantiatedSection);
            usedSections[instantiatedSection.GetFullId()].Add(instantiatedSection);
            instantiatedSection.gameObject.SetActive(true);
            instantiatedSection.OnSectionEnabled();
            EnsureEnoughUnusedPiecesExist();
        }

        private void EnsureEnoughUnusedPiecesExist()
        {
            foreach (RoadSection roadSection in prototypes.Values)
            {
                EnsureEnoughUnusedPiecesExist(roadSection);
            }

            EnsureEnoughTotalUnusedPiecesExist();
        }

        private void EnsureEnoughUnusedPiecesExist(RoadSection roadSection)
        {
            int currentCount = availableSections[roadSection.GetFullId()].Count;
            int moreToMake = minNumberOfUnusedRoadSectionsToCreateMore - currentCount;
            for (int i = 0; i < moreToMake; i++)
            {
                AddInstantiatedObject(CreateObject(prototypes[roadSection.GetFullId()]));
            }
        }

        private void EnsureEnoughTotalUnusedPiecesExist()
        {
            if (GetAllAvailableSections().Count() >= minNumberOfTotalUnusedRoadSectionsToGenerateMore) return;

            foreach (RoadSection prototype in prototypes.Values)
            {
                AddInstantiatedObject(CreateObject(prototype));
            }
        }

        public void ReleaseOldestInstantiatedSection()
        {
            RoadSection oldest = GetOldestSection();

            // stop here if we haven't found one
            if (oldest is null) return;

            // release
            ReleaseInstantiatedSection(oldest);
        }

        public RoadSection GetOldestSection()
        {
            RoadSection oldest = null;
            float oldestN = Mathf.Infinity;

            foreach (RoadSection roadSection in GetAllUsedSections())
            {
                if (oldestN <= roadSection.N) continue;
                oldestN = roadSection.N;
                oldest = roadSection;
            }
            return oldest;
        }

        public RoadSection GetNewestSection()
        {
            RoadSection candidate = null;
            float n = -Mathf.Infinity;

            foreach (RoadSection roadSection in GetAllUsedSections())
            {
                if (n >= roadSection.N) continue;
                n = roadSection.N;
                candidate = roadSection;
            }
            return candidate;
        }

        public void ReleaseInstantiatedSection(RoadSection instantiatedSection)
        {
            usedSections[instantiatedSection.GetFullId()].Remove(instantiatedSection);
            availableSections[instantiatedSection.GetFullId()].Add(instantiatedSection);
            instantiatedSection.gameObject.SetActive(false);
        }

        public IEnumerable<RoadSection> GetUniqueAvailableSections()
        {
            foreach (List<RoadSection> sections in availableSections.Values)
            {
                if (sections.Count == 0) continue;
                yield return sections[0];
                continue;
            }
        }

        public IEnumerable<RoadSection> GetAllAvailablePrototypes()
        {
            foreach (List<RoadSection> roadSections in availableSections.Values)
            {
                if (roadSections.Count == 0) continue;
                yield return prototypes[roadSections[0].GetFullId()];
            }
        }

        /// <summary>
        /// Oldest first
        /// </summary>
        public IEnumerable<RoadSection> GetAllUsedSectionsOrdered()
        {
            return GetAllUsedSections().OrderBy(section => section.N);
        }

        public IEnumerable<RoadSection> GetAllUsedSections()
        {
            return GetAllDictionaryValues(usedSections);
        }

        public IEnumerable<RoadSection> GetAllAvailableSections()
        {
            return GetAllDictionaryValues(availableSections);
        }

        private static IEnumerable<T> GetAllDictionaryValues<K, T>(Dictionary<K, List<T>> dict)
        {
            foreach (List<T> items in dict.Values)
            {
                foreach (T item in items) yield return item;
            }
        }

        public Dictionary<string, int> GetUsedSectionCountsByPieceTypeId()
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (string id in usedSections.Keys)
            {
                counts[id] = usedSections[id].Count;
            }
            return counts;
        }
    }
}