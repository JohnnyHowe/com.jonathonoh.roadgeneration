# ConvexHullCalculator Test Strategy

## Scope

This document outlines a test strategy for [ConvexHullCalculator.cs](/C:/Users/Work/Documents/Projects/drifto/drifto-unity/Assets/Packages/RoadGeneration/Scripts/ConvexShape2D/Runtime/ConvexHullCalculator.cs).

The target under test is `ConvexHullCalculator.GetConvexHull(IEnumerable<Vector2>)`, which:

- Sorts input points lexicographically.
- Uses Andrew's monotone chain algorithm to remove interior and collinear points.
- Returns hull vertices in clockwise order.
- Short-circuits for `0`, `1`, or `2` input points.

## Current Coverage

Existing tests in [TrivialCaseTests.cs](/C:/Users/Work/Documents/Projects/drifto/drifto-unity/Assets/Packages/RoadGeneration/Scripts/ConvexShape2D/Tests/ConvexHullCalculator/TrivialCaseTests.cs) cover:

- Empty input.
- Single-point input.
- Two-point input.

These tests validate the early-return branch only. They do not exercise the monotone chain logic, ordering guarantees, collinearity handling, or duplicate-point behavior.

## Test Objectives

The test suite should prove:

1. The returned hull contains exactly the extreme points needed to describe the convex boundary.
2. Interior points and redundant collinear edge points are excluded.
3. Output ordering matches the method contract: clockwise, starting from the leftmost point.
4. Results are stable for unsorted input, duplicate points, and negative or mixed coordinates.
5. Degenerate inputs either produce a sensible hull or expose undocumented behavior clearly.

## Recommended Test Areas

### 1. Canonical Shape Coverage

Use small deterministic point sets where the expected hull is obvious.

- Triangle with one interior point.
- Axis-aligned rectangle with interior points.
- Rotated rectangle / diamond shape.
- Concave point cloud where one point forms an inward notch.

Primary assertions:

- Hull contains only the outer vertices.
- Hull vertex count matches expectation.
- Returned order is clockwise.
- First vertex is the leftmost point. If multiple points share the same `x`, use the lower `y`.

### 2. Ordering and Input Independence

The algorithm sorts internally, so input order should not matter for `3+` points.

- Same point set supplied in random order.
- Same point set supplied already sorted.
- Same point set supplied in reverse order.

Primary assertions:

- Returned hull sequence is identical across permutations.
- Output still starts at the leftmost point and proceeds clockwise.

Note: this does not apply to the `<= 2` fast path, which currently returns the original input enumeration unchanged.

### 3. Collinearity Rules

The implementation removes points when `Cross(...) <= 0f`, so collinear middle points on the boundary should be discarded.

- All points collinear on a horizontal line.
- All points collinear on a diagonal line.
- Rectangle edge populated with extra collinear points between corners.

Primary assertions:

- For fully collinear input, clarify expected behavior and lock it down with tests.
- For partially collinear boundaries, only extreme endpoints for each straight edge remain.

This area is high value because collinearity is where monotone-chain implementations commonly diverge from intended behavior.

### 4. Duplicate Points

The implementation does not explicitly deduplicate input before hull construction.

- Duplicate corner points.
- Duplicate interior points.
- Input containing only repeated copies of one point.
- Input containing two unique points with duplicates.

Primary assertions:

- Duplicates do not create duplicate hull vertices for `3+` points.
- Behavior for `<= 2` unique points is explicitly documented by tests, even if it is not ideal.

### 5. Coordinate Variety

Use inputs that prevent accidental assumptions about quadrant or axis alignment.

- Negative coordinates.
- Mixed positive and negative coordinates.
- Large magnitude values within normal `float` ranges.
- Very small non-zero coordinate differences that are still well above `Mathf.Epsilon`.

Primary assertions:

- Hull geometry is correct regardless of coordinate sign or scale.

### 6. Contract and Invariant Tests

These tests should verify general properties rather than one exact fixture.

- Every returned point exists in the input set.
- Every input point lies on or inside the polygon described by the returned hull.
- Every consecutive triple of hull vertices turns consistently clockwise.
- No adjacent duplicate vertices appear in the result.

These are strong regression detectors when adding broader fixture coverage later.

## Suggested Test Structure

Organize tests by behavior rather than by helper method internals.

- `TrivialCaseTests`
- `CanonicalShapeTests`
- `OrderingTests`
- `CollinearityTests`
- `DuplicatePointTests`
- `InvariantTests`

Keep fixtures small and explicit. For each case, build the point set inline unless reuse materially improves readability.

## Assertion Strategy

Prefer exact sequence assertions for deterministic cases where the full expected hull is known.

Use helper assertions for reusable checks such as:

- `AssertHullEquals(expected, actual)`
- `AssertStartsAtLeftmostPoint(actual)`
- `AssertIsClockwise(actual)`
- `AssertContainsNoDuplicateVertices(actual)`

Avoid overly permissive set-based assertions unless order is intentionally irrelevant for that specific test.

## Risk-Based Priorities

If coverage is added incrementally, prioritize in this order:

1. Canonical shape tests.
2. Collinearity tests.
3. Duplicate-point tests.
4. Ordering and invariant tests.
5. Wider coordinate-range tests.

This order targets the parts of the implementation most likely to regress or hide subtle geometric defects.

## Open Questions To Resolve In Tests

Before expanding the suite, decide and document expected behavior for:

- Fully collinear inputs with more than two points.
- Duplicate-heavy inputs where the number of unique points is `<= 2`.
- Whether the "clockwise from leftmost point" contract is strict API behavior or an implementation detail.

If these expectations are not yet decided, write characterization tests first so future refactors do not silently change behavior.
