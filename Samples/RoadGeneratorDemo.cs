using System.Collections.Generic;
using System.Linq;
using JonathonOH.RoadGeneration;
using UnityEngine;

public class RoadGeneratorDemo : ARoadGenerator
{
    [SerializeField] private int _targetRoadLength = 10;
    [SerializeField] private float _timeBetweenPiecePlacing = 0.5f;
    private float _timeUntilNextPiece;

    private new void Awake()
    {
		NewSectionPlacedValue.AddListener(OnNewSectionPlaced);
		NoChoiceFound.AddListener(OnNoChoiceFound);
		PoolEmpty.AddListener(OnPoolEmpty);
        base.Awake();
        _timeUntilNextPiece = 0;
    }

    private new void Update()
    {
        base.Update();
        _timeUntilNextPiece -= Time.deltaTime;
    }

    protected override bool ShouldPlaceNewSection()
    {
        return _timeUntilNextPiece <= 0;
    }

    protected override bool ShouldRemoveLastSection()
    {
        return GetAllCurrentSections().Count() > _targetRoadLength;
    }

    protected void OnNewSectionPlaced(RoadSection section)
    {
        _timeUntilNextPiece += _timeBetweenPiecePlacing;
    }

    protected void OnNoChoiceFound()
    {
        Debug.Log("No choice found!");
    }

    protected void OnPoolEmpty()
    {
        Debug.Log("Pool is empty!");
    }

    protected override List<RoadSection> GetSectionsInPreferenceOrder(List<RoadSection> sectionPrototypes)
    {
        List<RoadSection> shuffled = new List<RoadSection>(sectionPrototypes);
        Shuffle(shuffled);
        return shuffled;
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
