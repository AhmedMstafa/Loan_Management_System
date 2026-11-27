---
name: feature-scaffolding
description: Steps for adding a new feature in a way that follows the project's architecture. Use before writing any new feature.
---

# New Feature Procedure

1. **Understand**: restate the goal in one sentence. If requirements are unclear, ask the most important questions ONE AT A TIME, then summarize the requirements before coding.
2. **Find the pattern**: locate the most similar existing feature and follow its structure and naming. Don't invent a new pattern.
3. **Plan (no code)**: list files to create/change, per layer, and the data flow. Wait for approval if it touches 3+ files.
4. **Implement in small steps**, one layer at a time (e.g. data → domain/service → UI), keeping each step compilable.
5. **Handle states**: loading, empty, error, success — all explicit.
6. **Register/wire**: routes, DI/container, config, API endpoint constants — whatever this project uses.
7. **Tests**: add tests for the business logic and data mapping (see `.ai/agents/test-writer.md`).
8. **Verify**: try to break it — bad input, unexpected API data, timeouts, double-submit. Fix what you find.
9. **Finish**: run `.ai/skills/code-review.md`.

## Feature Checklist
- [ ] Follows an existing feature's structure
- [ ] No layer boundary violations
- [ ] All UI states handled
- [ ] Wired up (routes / DI / endpoints)
- [ ] Tests added
- [ ] No new dependency without justification
