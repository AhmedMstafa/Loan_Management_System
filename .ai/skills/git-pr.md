---
name: git-pr
description: Branch name, commit message, and PR description conventions. Use after code review passes.
---

# Git & Pull Request Output

## Gather context
- `git status` + `git diff`
- **Infer** ticket number from branch name / task context / recent commits; ask only if you truly can't
- **Infer** base branch from the repo default (`git symbolic-ref refs/remotes/origin/HEAD`); ask only if ambiguous
- Pick type: `feat | fix | refactor | perf | chore | test | docs`

## Branch
`{type}/{ticket}-short-description` (or `{type}/short-description` with no ticket). Lowercase, hyphenated.

## Commit (Conventional Commits)
```
<type>(<scope>): <summary>

<optional body — why, not what>
```
Summary: imperative, no period, ≤ 72 chars.

## PR description (Markdown, concise)
- First line: `[TICKET-123](url)` — omit entirely if no ticket
- `## Summary` — 1–3 sentences: what and why
- `## Changes` — a few tight bullets
- `## Root Cause` — bug fixes only
- `## Testing` — how it was verified
- Omit empty sections. No AI-attribution footer. Don't restate every file.

## Rules
- **Ask for confirmation before pushing.**
- Never force-push or discard work without explicit approval.
