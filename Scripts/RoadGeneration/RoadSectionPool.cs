using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSectionPool
    {
        private int objectsPerSection;
        // id, pieces
        public Dictionary<string, List<RoadSection>> availableSections;
        public Dictionary<string, List<RoadSection>> usedSections;
        public Dictionary<string, RoadSection> prototypes;
        private Transform container;

        public RoadSectionPool(List<RoadSection> uninstantiatedRoadSections, Transform gameObjectContainer, int objectsPerSection)
        {
            container = gameObjectContainer;
            this.objectsPerSection = objectsPerSection;
            CreatePrototypes(uninstantiatedRoadSections);
            CreateObjects();
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

            RoadSection instantiatedSection = gameObject.GetComponent<RoadSection>();
            gameObject.name = instantiatedSection.GetFullId();
            return instantiatedSection;
        }

        public void AddInstantiatedObject(RoadSection instantiatedSection)
        {
            instantiatedSection.OnPoolObjectCreated();
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
                //TODO add createIfNoneAvailable
                throw new Exception("No available section");
            }

            return roadSections[0];
        }

        public void ClaimInstantiatedSection(RoadSection instantiatedSection)
        {
            availableSections[instantiatedSection.GetFullId()].Remove(instantiatedSection);
            usedSections[instantiatedSection.GetFullId()].Add(instantiatedSection);
            instantiatedSection.gameObject.SetActive(true);
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

        public IEnumerable<RoadSection> GetAllUsedSectionsOrdered()
        {
            return GetAllUsedSections().OrderBy(section => section.N);
        }

        public IEnumerable<RoadSection> GetAllUsedSections()
        {
            return GetAllDictionaryValues(usedSections);
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