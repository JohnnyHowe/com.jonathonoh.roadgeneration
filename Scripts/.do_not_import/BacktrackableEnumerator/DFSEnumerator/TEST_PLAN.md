# DFSEnumerator Test Plan

This plan is based on intended functionality, not the current implementation.

## Intended Contract

`DFSEnumerator` should enumerate a search space using depth-first traversal while allowing callers to backtrack from failed or rejected branches.

The intended behavior is:

- Visit candidates in deterministic depth-first order.
- Expose each candidate exactly when it becomes the active search choice.
- Allow the caller to accept a candidate and continue deeper.
- Allow the caller to reject or backtrack from a candidate and continue with the next valid sibling.
- Stop cleanly when every reachable branch has been explored.
- Preserve traversal state across repeated enumeration calls.
- Avoid revisiting exhausted branches.
- Handle empty, single-node, shallow, and deeply nested search spaces.
- Propagate invalid usage clearly instead of silently corrupting traversal state.

## Scope

Unit tests should cover the enumerator as a reusable traversal primitive. They should use small deterministic fake search graphs rather than road-generation-specific fixtures.

Integration tests can cover road-generation consumers later, but those tests should verify consumer behavior rather than redefining the DFS contract.

## Test Fixtures

Use a simple in-memory node model:

- `Id`: stable label for assertions.
- `Children`: ordered list of child nodes.
- Optional `IsValid` or callback-controlled decision result for pruning tests.

Prefer named graphs over random generation for unit tests. The assertion value should usually be the emitted node id sequence.

Example graph:

```text
A
|- B
|  |- D
|  `- E
`- C
   `- F
```

Expected full DFS order: `A, B, D, E, C, F`.

## Core Test Cases

### Construction

- Creates an enumerator for a non-empty root without advancing traversal.
- Creates an enumerator for an empty search space and completes immediately.
- Rejects null root, null child provider, or other required null dependencies with a clear exception.

### Basic DFS Enumeration

- Enumerates a single root once, then completes.
- Enumerates a flat root with multiple children in declared child order.
- Enumerates a balanced tree in preorder depth-first order.
- Enumerates an unbalanced tree without skipping deeper descendants.
- Completes only after the final reachable node has been emitted.
- Returns a stable completion signal after exhaustion if advanced again.

### State Preservation

- Multiple calls advance one step at a time without restarting.
- Current item remains stable until the next successful advance.
- Exhausted traversal does not mutate the last valid result unexpectedly.
- Two independent enumerator instances over the same graph do not share traversal state.

### Backtracking

- Backtracking from a leaf advances to the next sibling.
- Backtracking from an internal node skips that node's remaining descendants.
- Backtracking repeatedly climbs until a sibling branch is available.
- Backtracking from the root exhausts traversal.
- Backtracking after traversal is already exhausted is handled consistently.
- Backtracking before the first advance is rejected or treated as a documented no-op.

### Branch Pruning

- Rejected candidate is not returned again.
- Rejected internal node prevents its descendants from being visited.
- Accepted candidate allows traversal into its descendants.
- Mixed accept/reject decisions produce the expected remaining DFS order.

Example:

```text
A
|- B
|  |- D
|  `- E
`- C
   `- F
```

If `B` is rejected, expected emitted accepted path order should continue with `C, F`; `D` and `E` should never be emitted.

### Ordering Guarantees

- Children are explored in the exact order supplied by the child provider.
- After a subtree is exhausted, traversal continues with the nearest unvisited sibling.
- No breadth-first behavior appears in mixed-depth graphs.

### Edge Cases

- Empty child lists are treated as leaves.
- Duplicate node values are emitted as distinct positions if they are distinct search candidates.
- Deep chains complete without losing order.
- Wide sibling sets complete without skipping or duplicating candidates.
- Cyclic input is either rejected by contract or protected by caller-provided cycle handling; whichever contract is chosen must be documented and tested.

### Error Handling

- Exceptions from the child provider propagate with traversal state left in a documented condition.
- Exceptions from caller decision callbacks do not cause hidden extra advances.
- Invalid method call order produces documented exceptions or documented no-op behavior.

## Suggested Test Names

- `MoveNext_WhenGraphIsEmpty_ReturnsFalse`
- `MoveNext_WhenSingleRoot_EmitsRootThenCompletes`
- `MoveNext_WhenBalancedTree_EmitsPreorderDepthFirstSequence`
- `MoveNext_WhenUnbalancedTree_VisitsDeepBranchBeforeSibling`
- `Backtrack_WhenAtLeaf_MovesToNextSibling`
- `Backtrack_WhenAtInternalNode_SkipsDescendants`
- `Backtrack_WhenNoSiblingExists_ClimbsToNearestAvailableSibling`
- `Reject_WhenCandidateHasChildren_DoesNotVisitDescendants`
- `MoveNext_AfterExhaustion_RemainsExhausted`
- `TwoEnumerators_WhenUsingSameGraph_DoNotShareState`

## Acceptance Criteria

The test suite is sufficient when it proves:

- Deterministic preorder DFS traversal.
- Correct caller-controlled backtracking.
- Correct pruning of rejected branches.
- Stable behavior at start, during traversal, and after exhaustion.
- Clear handling of invalid inputs and invalid call order.

## Out Of Scope

- Performance testing for large road networks.
- Road layout quality.
- Unity scene object creation.
- Randomized procedural generation behavior.
- Behavior that depends on the current implementation rather than the intended DFS/backtracking contract.
