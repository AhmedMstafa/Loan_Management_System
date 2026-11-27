---
name: code-review
description: Self-review checklist to run before marking any task done. Read-only — do not modify code during this step.
---

# Code Completion Self-Review

Run on the current changes (`git diff`, `git status`). Do NOT edit code here; report and propose fixes.

## Correctness
- [ ] Root cause addressed, not just the symptom
- [ ] Solves the specific problem in the task
- [ ] Edge cases: null, empty, loading, error
- [ ] No silent failures; errors propagate cleanly

## Architecture
- [ ] Layer boundaries respected
- [ ] No business logic in UI
- [ ] Shared logic is in the shared module, not duplicated
- [ ] Any new abstraction is justified

## Safety
- [ ] No existing behavior, API, or UX broken
- [ ] No performance regressions (expensive work in hot paths, unnecessary re-renders/re-queries)
- [ ] No hardcoded secrets, unvalidated input, or sensitive data in logs
- [ ] No unused imports, dead code, or debug leftovers
- [ ] Resources are released (listeners, subscriptions, connections, timers)

## Quality
- [ ] Readable, follows project conventions and naming
- [ ] Files/functions small and focused
- [ ] Tests exist for the changed logic

## Output
1. **What** changed  2. **Why**  3. **Why it's safe and correct**
If any item fails: flag it and propose a fix before continuing.
