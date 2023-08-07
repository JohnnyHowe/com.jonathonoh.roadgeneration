using System.Collections;
using System.Collections.Generic;
using JonathonOH.RoadGeneration;
using UnityEngine;

public class RoadGeneratorDemo : ARoadGenerator
{
    [SerializeField] private int _targetRoadLength = 10;
    [SerializeField] private float _timeBetweenPiecePlacing = 0.5f;
    [SerializeField] private float _maxTimeBetweenRemovingPieces = 1f;
    private float _timeUntilNextPiece;
    private float _timeUntilRemovingNextPiece;

    private new void Awake()
    {
        base.Awake();
        _timeUntilNextPiece = 0;
        _timeUntilRemovingNextPiece = 0;
    }

    private new void Update()
    {
        base.Update();
        _timeUntilNextPiece -= Time.deltaTime;
        _timeUntilRemovingNextPiece -= Time.deltaTime;
    }

    protected override bool ShouldPlaceNewPiece()
    {
        return _timeUntilNextPiece <= 0;
    }

    protected override bool ShouldRemoveLastPiece()
    {
        return currentPieces.Count > _targetRoadLength || _timeUntilRemovingNextPiece <= 0;
    }

    protected override void OnNewPiecePlaced(RoadSection section)
    {
        _timeUntilNextPiece += _timeBetweenPiecePlacing;
        _timeUntilRemovingNextPiece = _maxTimeBetweenRemovingPieces;
    }

    protected override void OnPieceRemoved()
    {
        _timeUntilNextPiece = _timeBetweenPiecePlacing;
        _timeUntilRemovingNextPiece += _maxTimeBetweenRemovingPieces;
    }

    protected override List<RoadSection> GetPiecesInPreferenceOrder(List<RoadSection> sectionPrototypes)
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
