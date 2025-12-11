# Overview
This tool streamlines work with Git submodules inside multiple related repositories (“superprojects”).

## Terminology
- **Superproject** — Main repository that includes one or more submodules.  
- **Submodule** — Git repository referenced by a superproject.

The tool provides two core operations:
1. **Submodule Index Validation** — Checks whether superprojects reference the `latest commit` of a given `submodule` on a given `branch`.
2. **Submodule Index Alignment Across Superprojects** — Updates all superprojects to reference the `HEAD submodule commit`.

# User Settings / Initial Configuration
To use the tool, absolute paths to all relevant superprojects must be configured in the **User Settings** screen.

- **Superprojects** — List of superprojects on which operations may be executed.  
- **PushToRemote** — If `true`, the Alignment operation may push aligned states to the remote (user confirmation required).

# Operations

## Submodule Index Validation (Idempotent)
Validates, whether submodule commit index references the head commit index on given branch.

The operation can run:
- On a single superproject  
- On all configured superprojects  

Parameters:
- **SubmoduleX** — The submodule to validate  
- **BranchY** — The branch whose expected submodule state will be compared  

Validation rule:
Superproject.BranchX.HEAD.SubmoduleXCommit == SubmoduleX.BranchY.HEAD.Commit

Only **remote state** is evaluated. Local unpushed changes are ignored.

---

## Submodule Alignment Across Superprojects
Forwards reference to the `new version` of the `submodule` in all relevant `Superprojects`.

### Algorithm
**I. Finding Misalignments**\
Executes Submodule Index Validation for `selected submodule` on `test` branch across all `superprojects in configuration`

**II. Alignment Execution**\
For each misaligned superproject:
1. Switch superproject to `test`
2. Fetch + fast-forward pull
3. Switch submodule to `test`
4. Fetch + fast-forward pull
5. Create a forward commit in the superproject with the updated submodule reference
6. Switch superproject to `dev`
7. Merge `test` into `dev`

This operation is intended **after a feature is completed and merged**, since merged features are expected to be present in the `test` branch.

### Handled cases
- Target branch does not exist locally (superproject or submodule)  
  → Created automatically via `git switch`
- Fast-forward pull fails (conflict or diverged history)  
  → `FastForwardMergeFailureException`

### Known Issue: Local alignment vs remote misalignment
Occurs when:
- A developer aligns the state locally (manually or using this tool),  
- The remote remains outdated,  
- And the alignment operation is executed again.

The tool detects remote misalignment and attempts local alignment, but the local state is already correct → `ForwardCommitCreationException`.
