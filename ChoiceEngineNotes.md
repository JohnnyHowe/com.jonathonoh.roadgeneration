# Choice Engine Notes

## Current Problems

### 1. Opaque mutable state

`RoadGeneratorChoiceEngine` stores search inputs and search progress internally:

- `sectionsInWorld`
- `sectionPrototypes`
- `_combinationGenerator`

That makes the engine harder to debug because behavior depends on hidden internal state instead of explicit inputs and outputs. Reproducing a failure requires stepping the object through the same lifecycle rather than rerunning a deterministic function.

Relevant code:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:16)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:23)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:42)

### 2. Tight coupling to scene objects

The engine works directly with `RoadSection` MonoBehaviours and calls `GetShape()` during search. That couples search behavior to Unity object state, cached shape state, transform correctness, and prefab setup.

This is the main reason the engine is difficult to unit test in isolation.

Relevant code:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:105)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:117)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:157)

### 3. Weak diagnostics

The current debugging strategy is mostly `Debug.Log` calls inside the search loop. That produces noisy output but not structured information that can be inspected later.

Relevant code:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:25)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:53)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:56)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:63)

### 4. Mixed responsibilities

The engine currently does all of the following:

- owns the DFS search state
- derives candidate chains
- aligns candidate shapes
- checks overlap
- decides success or failure
- logs debug output

That makes it hard to reason about failures because there is no clear seam between "search", "placement", and "collision checking".

### 5. Duplicate and inconsistent start-point logic

The choice engine computes its own initial candidate start transform, while `ARoadGenerator` also computes a start transform separately.

Relevant code:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:135)
- [ARoadGenerator.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\ARoadGenerator.cs:110)

This should be a single responsibility owned by one source of truth, not duplicated across planner and generator.

### 6. Hidden search budget and weak failure modes

The engine uses a hardcoded `MAX_ITERATIONS = 10000000` limit. If search fails, the caller does not get a structured reason such as:

- no valid path exists
- search budget exhausted
- invalid input state

Relevant code:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:20)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:32)

### 7. Implicit invariants

`_DoesLastCandidateSectionOverlapWithOthers()` assumes there is at least one aligned candidate and indexes the last item directly.

Relevant code:

- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:70)
- [RoadGeneratorChoiceEngine.cs](C:\Users\Work\Documents\Projects\drifto\drifto-unity\Assets\Packages\RoadGeneration\Scripts\RoadGeneration\RoadGeneratorChoiceEngine.cs:72)

That should be an explicit invariant with a clear error path.

## Recommended Direction

The main design change is to make the choice engine a pure planner instead of a stateful runtime object.

### Proposed API shape

```csharp
public sealed class RoadChoicePlanner
{
    public RoadChoiceResult FindFirstValidChoice(RoadChoiceRequest request);
}

public sealed class RoadChoiceRequest
{
    public IReadOnlyList<RoadShapePlacement> CurrentSections { get; init; }
    public IReadOnlyList<RoadSectionDefinition> CandidateSections { get; init; }
    public TransformData NextStart { get; init; }
    public int CheckDepth { get; init; }
    public int IterationBudget { get; init; }
}

public sealed class RoadChoiceResult
{
    public bool Success { get; init; }
    public string ChosenPrototypeId { get; init; }
    public int IterationCount { get; init; }
    public RoadChoiceFailureReason FailureReason { get; init; }
    public IReadOnlyList<SearchTraceStep> Trace { get; init; }
}
```

The important point is not the exact names. The important point is that the request fully describes the problem, and the result fully describes the outcome.

## Suggested Internal Split

### `RoadChoicePlanner`

Top-level planner API. Accepts immutable input and returns a result object.

### `RoadChoiceSearchState`

Encapsulates DFS indices and search progress. This replaces direct exposure of `_combinationGenerator`.

### `CandidateChainBuilder`

Builds aligned placements for a candidate chain from:

- current placed sections
- candidate definitions
- a starting transform

### `RoadOverlapChecker`

Pure overlap logic between already-aligned shapes.

### `RoadChoiceResult`

Structured success or failure result, including diagnostic data.

## Debugging Improvements

Replace log spam with structured trace data.

Suggested trace fields:

- attempted candidate ids
- aligned start/end transforms per candidate
- overlap pair that caused rejection
- rejection reason
- iteration count
- whether failure was caused by impossibility or budget exhaustion

This can still be logged in editor builds, but the primary artifact should be a data structure rather than side effects.

## Practical Benefits

### Easier unit testing

A pure planner can be tested with plain data. That avoids requiring instantiated `GameObject`s and `MonoBehaviour`s just to validate search behavior.

### Better reproducibility

If a bad choice or failure occurs, the exact `RoadChoiceRequest` can be serialized or reconstructed and replayed.

### Clearer ownership

The generator decides when to ask for a choice.
The planner decides what choice is valid.
The pool decides how to instantiate the chosen section.

### Better editor tooling

Once the planner exposes trace data, an editor window or gizmo overlay can show:

- the rejected chains
- the first overlap encountered
- the winning path

## Concrete Issues To Fix Even Before Full Refactor

These are worth fixing even if the larger rewrite is deferred.

1. Remove profanity and unstructured logs from the search loop.
2. Make iteration budget configurable instead of hardcoded.
3. Pass initial start transform into the engine instead of recomputing it internally.
4. Guard against empty aligned candidate lists with explicit validation.
5. Stop exposing `_combinationGenerator` publicly.
6. Return explicit failure reasons instead of relying on exceptions and log output.

## Suggested Test Cases

Once the planner is data-driven, add tests for:

1. First candidate succeeds with no overlap.
2. First candidate fails, second candidate succeeds.
3. Failure caused by overlap on the second depth step.
4. Failure because no valid chain exists.
5. Failure because iteration budget is exhausted.
6. Empty world state with explicit starting transform.
7. Existing world state with non-identity end transform.

## Refactor Order

1. Introduce request/result types around the existing engine.
2. Move start-transform selection out of the engine.
3. Replace `RoadSection` search inputs with plain definitions or shape snapshots.
4. Extract overlap checking into a pure helper.
5. Replace direct logging with structured trace capture.
6. Remove mutable engine lifecycle methods like `Reset()` in favor of one-shot planning calls.
