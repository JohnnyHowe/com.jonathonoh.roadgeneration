# Overlap Logic Refactor Notes

## Goal

Extract overlap checking from `RoadGeneratorChoiceEngine` into a small, testable, pure component with clear inputs and outputs.

The overlap logic should not need to know about:

- DFS search state
- `MonoBehaviour` lifecycle
- pool state
- logging policy
- placement timing

It should only answer:

1. what shapes are being compared
2. whether they overlap
3. which pair caused failure

## Current Coupling

Today the overlap logic is embedded in `RoadGeneratorChoiceEngine`:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:66)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:98)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:108)

That method currently depends on several hidden steps:

1. read current world sections
2. derive candidate sections from DFS state
3. align candidate section shapes
4. collect current and candidate shapes into one list
5. compare the newest candidate against all previous shapes

This makes overlap behavior hard to debug because "alignment" and "collision checking" are currently fused together.

## Refactor Principle

Split the concern into two layers:

### 1. Placement/alignment layer

Responsible for producing aligned shapes in world space.

Input:

- current placed shapes
- candidate section definitions
- a starting transform

Output:

- aligned candidate placements

### 2. Overlap layer

Responsible only for comparing already-aligned shapes.

Input:

- a list of already-aligned shapes
- or one aligned shape plus a list of others

Output:

- overlap result with details

This lets you test collision rules independently from transform alignment rules.

## Recommended Target API

Start with a very small API.

```csharp
public sealed class RoadOverlapChecker
{
    public OverlapCheckResult CheckNewestAgainstPrevious(
        IReadOnlyList<RoadSectionShape> alignedShapes);

    public OverlapCheckResult CheckOneAgainstMany(
        RoadSectionShape candidate,
        IReadOnlyList<RoadSectionShape> existingShapes);
}

public readonly struct OverlapCheckResult
{
    public bool HasOverlap { get; init; }
    public int CandidateIndex { get; init; }
    public int OtherIndex { get; init; }
    public RoadSectionShape Candidate { get; init; }
    public RoadSectionShape Other { get; init; }
}
```

The first method matches your current engine behavior closely.
The second method is the more generally useful primitive.

## Better Data Boundary

Right now overlap checks operate on `RoadSectionShape`, which is reasonable as a first extraction step.

That means the first refactor does not need to redesign all geometry types. It can simply:

- keep `RoadSectionShape.DoesOverlapWith(...)`
- move iteration/comparison logic into a dedicated checker

Later, if needed, you can introduce a thinner geometry DTO, but that is not necessary for the first step.

## First Extraction Step

Create a class whose only job is to compare shapes:

```csharp
public sealed class RoadOverlapChecker
{
    public OverlapCheckResult CheckOneAgainstMany(
        RoadSectionShape candidate,
        IReadOnlyList<RoadSectionShape> existingShapes)
    {
        for (int i = 0; i < existingShapes.Count; i++)
        {
            if (!candidate.DoesOverlapWith(existingShapes[i]))
            {
                continue;
            }

            return new OverlapCheckResult
            {
                HasOverlap = true,
                Candidate = candidate,
                Other = existingShapes[i],
                OtherIndex = i
            };
        }

        return new OverlapCheckResult
        {
            HasOverlap = false,
            Candidate = candidate,
            OtherIndex = -1
        };
    }
}
```

At this stage:

- `RoadGeneratorChoiceEngine` still builds aligned shapes
- the checker only evaluates overlap

That is already a meaningful improvement because overlap behavior becomes directly unit-testable.

## Second Extraction Step

Once shape comparison is extracted, separate alignment from search.

Introduce something like:

```csharp
public sealed class CandidateChainBuilder
{
    public IReadOnlyList<RoadSectionShape> BuildAlignedCandidateShapes(
        TransformData start,
        IReadOnlyList<RoadSectionShape> candidateShapes);
}
```

Or, if you want to preserve identity:

```csharp
public sealed class AlignedRoadSection
{
    public string SectionId { get; init; }
    public int ChainIndex { get; init; }
    public RoadSectionShape Shape { get; init; }
}
```

Then the engine flow becomes:

1. ask DFS for a candidate chain
2. build aligned candidate shapes
3. compare newest candidate against previous shapes
4. mark the DFS state valid or invalid

That is much clearer than the current combined method.

## Suggested Class Responsibilities

### `RoadOverlapChecker`

Pure collision comparison logic.

It should:

- compare aligned shapes
- return structured overlap details

It should not:

- derive candidates from DFS state
- log to the console
- know about `RoadSection`
- know about the pool or generator

### `CandidateChainBuilder`

Pure alignment logic.

It should:

- take ordered candidate definitions or shapes
- align them from a known start transform
- return world-aligned shapes

It should not:

- decide whether overlap means search failure
- mutate search state

### `RoadGeneratorChoiceEngine`

Search orchestration only.

It should:

- request current candidate chain from DFS
- call the chain builder
- call the overlap checker
- tell DFS whether the chain is valid so far

It should not:

- implement shape comparison loops directly

## Minimal Change Version

If you want the lowest-risk refactor, do only this:

1. Move `_DoesLastCandidateSectionOverlapWithOthers()` into `RoadOverlapChecker`.
2. Rename it to something explicit like `CheckNewestAgainstPrevious`.
3. Pass in `List<RoadSectionShape>` instead of letting the checker read engine state.
4. Return a result object instead of `bool`.

That gives you better naming, clearer ownership, and better diagnostics without forcing a broader rewrite.

## Recommended Result Type

Use a structured result instead of `bool`.

```csharp
public enum OverlapFailureKind
{
    None,
    CandidateOverlapsExisting
}

public readonly struct OverlapCheckResult
{
    public OverlapFailureKind FailureKind { get; init; }
    public int CandidateIndex { get; init; }
    public int OtherIndex { get; init; }
    public bool HasOverlap => FailureKind != OverlapFailureKind.None;
}
```

That makes later debugging and visualization easier.

## Debugging Benefits

After extraction, if a candidate is rejected you can inspect:

- candidate chain index
- which prior shape caused the rejection
- the exact candidate shape
- the exact prior shape

That gives you much better failure information than a generic `"Most recent section overlaps"` log.

## Suggested Test Cases

Add unit tests directly against `RoadOverlapChecker`.

1. candidate does not overlap any previous shape
2. candidate overlaps the first previous shape
3. candidate overlaps a later previous shape
4. empty previous-shape list returns no overlap
5. `CheckNewestAgainstPrevious` rejects invalid empty input clearly

You can write these tests without involving DFS or scene objects if you build simple `RoadSectionShape` instances directly.

## Incremental Refactor Order

1. Extract a new `RoadOverlapChecker` class.
2. Move shape comparison loop into that class.
3. Replace `bool` return values with `OverlapCheckResult`.
4. Update `RoadGeneratorChoiceEngine` to pass aligned shapes into the checker.
5. Add unit tests for overlap behavior only.
6. Optionally extract candidate alignment into `CandidateChainBuilder`.

## End State

The end state should look roughly like this:

```text
RoadGeneratorChoiceEngine
  -> DFS state provider
  -> CandidateChainBuilder
  -> RoadOverlapChecker

RoadOverlapChecker
  -> RoadSectionShape.DoesOverlapWith(...)
```

That keeps overlap logic as a narrow geometry concern instead of burying it inside search orchestration.
