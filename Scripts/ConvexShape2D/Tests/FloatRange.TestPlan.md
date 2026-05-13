# FloatRange Test Plan

## Scope

This plan covers intended behavior for `Assets/Packages/RoadGeneration/Scripts/ConvexShape2D/Runtime/FloatRange.cs`.

`FloatRange` represents an inclusive 1D range used by `Shape.GetProjection` and `Shape.DoesOverlapWith` in the Separating Axis Theorem flow. Tests should validate the public contract of the type, not preserve current bugs.

## Intended Contract

From the implementation and surrounding usage, `FloatRange` is intended to provide:

- Automatic normalization of the two input values so `Min <= Max`.
- Mutable reset behavior through `Set`.
- Inclusive overlap checks, where touching at a boundary still counts as overlap.
- Value equality based on normalized `Min` and `Max`.
- Equality operators consistent with `Equals` and `GetHashCode`.

## Out Of Scope

- Floating-point tolerance behavior. The current API uses exact `float` equality, so tests should reflect exact comparisons unless the contract is intentionally changed later.
- Performance or allocation testing.
- `DoesOverlapWith(null)` behavior. There is no clear API contract for this yet, so this should be decided before writing a defensive test.

## Test Areas

### 1. Construction And Normalization

Purpose: verify that the constructor always stores the lower value in `Min` and the higher value in `Max`.

Planned tests:

- `Constructor_WhenValuesAreAscending_PreservesMinAndMax`
  - Example: `(1f, 3f)` produces `Min = 1f`, `Max = 3f`.
- `Constructor_WhenValuesAreDescending_NormalizesMinAndMax`
  - Example: `(3f, 1f)` produces `Min = 1f`, `Max = 3f`.
- `Constructor_WhenValuesAreEqual_CreatesSinglePointRange`
  - Example: `(2f, 2f)` produces `Min = 2f`, `Max = 2f`.
- `Constructor_WhenValuesIncludeNegativeNumbers_NormalizesCorrectly`
  - Example: `(-2f, -5f)` produces `Min = -5f`, `Max = -2f`.
- `Constructor_WhenRangeCrossesZero_NormalizesCorrectly`
  - Example: `(4f, -1f)` produces `Min = -1f`, `Max = 4f`.

### 2. Set Behavior

Purpose: verify that `Set` reuses the same normalization rules and fully updates prior state.

Planned tests:

- `Set_WhenValuesAreAscending_StoresExactBounds`
- `Set_WhenValuesAreDescending_NormalizesBounds`
- `Set_WhenCalledMultipleTimes_ReplacesPreviousBounds`
  - Example: construct `(1f, 3f)`, then `Set(10f, 4f)`, expect `Min = 4f`, `Max = 10f`.
- `Set_WhenValuesAreEqual_CreatesSinglePointRange`

### 3. Overlap Semantics

Purpose: verify the inclusive overlap behavior needed by SAT projection testing.

Planned tests:

- `DoesOverlapWith_WhenRangesAreIdentical_ReturnsTrue`
  - Example: `[1, 3]` with `[1, 3]`.
- `DoesOverlapWith_WhenOneRangeIsContainedWithinAnother_ReturnsTrue`
  - Example: `[1, 5]` with `[2, 4]`.
- `DoesOverlapWith_WhenRangesPartiallyOverlapOnLeft_ReturnsTrue`
  - Example: `[1, 4]` with `[0, 2]`.
- `DoesOverlapWith_WhenRangesPartiallyOverlapOnRight_ReturnsTrue`
  - Example: `[1, 4]` with `[3, 6]`.
- `DoesOverlapWith_WhenRangesTouchAtBoundary_ReturnsTrue`
  - Example: `[1, 3]` with `[3, 5]`.
  - This is important because SAT contact should count as overlap, not separation.
- `DoesOverlapWith_WhenRangesAreSeparated_ReturnsFalse`
  - Example: `[1, 2]` with `[3, 4]`.
- `DoesOverlapWith_WhenSeparatedByNegativeAndPositiveValues_ReturnsFalse`
  - Example: `[-5, -1]` with `[0, 2]`.
- `DoesOverlapWith_WhenCalledInEitherDirection_ReturnsSameResult`
  - Verifies symmetry: `a.DoesOverlapWith(b) == b.DoesOverlapWith(a)`.
- `DoesOverlapWith_WhenPointRangeLiesInsideOtherRange_ReturnsTrue`
  - Example: `[2, 2]` with `[1, 3]`.
- `DoesOverlapWith_WhenTwoPointRangesShareSameValue_ReturnsTrue`
  - Example: `[2, 2]` with `[2, 2]`.
- `DoesOverlapWith_WhenTwoPointRangesDiffer_ReturnsFalse`
  - Example: `[2, 2]` with `[3, 3]`.

### 4. Equality Contract

Purpose: verify standard value-object equality semantics for a normalized range.

Planned tests:

- `Equals_WhenRangesHaveSameNormalizedBounds_ReturnsTrue`
  - Example: `(1f, 3f)` equals `(1f, 3f)`.
- `Equals_WhenRangesAreConstructedWithReversedInputs_ReturnsTrue`
  - Example: `(1f, 3f)` equals `(3f, 1f)`.
- `Equals_WhenMinDiffers_ReturnsFalse`
- `Equals_WhenMaxDiffers_ReturnsFalse`
- `Equals_WhenComparingSameInstance_ReturnsTrue`
- `Equals_WhenOtherIsNull_ReturnsFalse`
  - Intended standard behavior for `IEquatable<T>`.
- `EqualsObject_WhenObjectIsSameValue_ReturnsTrue`
- `EqualsObject_WhenObjectIsDifferentType_ReturnsFalse`
- `EqualsObject_WhenObjectIsNull_ReturnsFalse`
- `Equality_IsReflexiveSymmetricAndTransitive`
  - Use three equivalent ranges to assert the core equality contract.

### 5. Operator Semantics

Purpose: ensure `==` and `!=` remain aligned with `Equals`.

Planned tests:

- `OperatorEquality_WhenBothSidesNull_ReturnsTrue`
- `OperatorEquality_WhenLeftIsNullAndRightIsNot_ReturnsFalse`
- `OperatorEquality_WhenRightIsNullAndLeftIsNot_ReturnsFalse`
- `OperatorEquality_WhenRangesHaveSameNormalizedBounds_ReturnsTrue`
- `OperatorEquality_WhenRangesDiffer_ReturnsFalse`
- `OperatorInequality_WhenRangesHaveSameNormalizedBounds_ReturnsFalse`
- `OperatorInequality_WhenRangesDiffer_ReturnsTrue`
- `OperatorEquality_MatchesEqualsResult`

### 6. Hash Code Contract

Purpose: ensure hash code behavior is valid for use in sets and dictionaries.

Planned tests:

- `GetHashCode_WhenRangesAreEqual_ReturnsSameHashCode`
  - Example: `(1f, 3f)` and `(3f, 1f)`.
- `GetHashCode_WhenCalledRepeatedlyForSameRange_IsStable`

Note: tests should not require unequal objects to produce different hash codes.

## Suggested Test Order

1. Construction and `Set`.
2. Overlap semantics.
3. Equality and operators.
4. Hash code contract.

This order should expose the highest-value functional issues first and keeps later tests easier to diagnose.

## Likely Test Fixture Structure

- `FloatRangeTests`
  - Construction and `Set` tests.
  - Overlap tests.
  - Equality/operator/hash tests.

Given the class size, a single fixture is probably enough unless the suite becomes hard to scan.

## Open Questions Before Implementing The Full Suite

- Should `DoesOverlapWith` explicitly throw on `null`, or is `null` considered invalid input that tests should ignore?
- Should future versions support approximate floating-point equality, or is exact equality part of the intended contract?

These do not block the core suite above, except for a possible null-input test on `DoesOverlapWith`.
