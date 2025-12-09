# Overview
This tool streamlines work with Git submodules inside multiple related repositories (“superprojects”).

## Terminology
- **Superproject** — Main repository that includes one or more submodules.  
- **Submodule** — Git repository referenced by a superproject.

The tool provides two core operations:
1. **Submodule Index Validation** — Checks whether superprojects reference the correct commit of a given submodule.
2. **Submodule Index Alignment Across Superprojects** — Updates all superprojects to reference the same (new) submodule commit.

# User Settings / Initial Configuration
To use the tool, absolute paths to all relevant superprojects must be configured in the **User Settings** screen.

- **Superprojects** — List of superprojects on which operations may be executed.  
- **PushToRemote** — If `true`, the Alignment operation may push aligned states to the remote (user confirmation required).

# Operations

## Submodule Index Validation (Idempotent)
Checks whether the commit index recorded in a superproject matches the expected commit index in the submodule.

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
Aligns the selected submodule to a specific commit across all superprojects where validation fails.

**Algorithm (only the `test` branch is considered):**
For each superproject:
1. Switch superproject to `test`
2. Fetch + fast-forward pull
3. Switch submodule to `test`
4. Fetch + fast-forward pull
5. Create a forward commit in the superproject with the updated submodule reference

This operation is intended **after a feature is completed and merged**, since merged features are expected to be present on `test`.

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
