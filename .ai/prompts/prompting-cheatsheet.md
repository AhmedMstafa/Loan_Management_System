# Prompting Cheat Sheet (works with any AI agent)

1. **Not a Google search** — brief a smart colleague who knows nothing about your project: problem, expected result, code.
2. **First sentence = the goal.** Context after.
3. **Persistent context goes in AGENTS.md**, not in every prompt.
4. **Show, don't tell** — give 2–3 examples of the exact format.
5. **One prompt, one job** — chain small prompts (design → data → logic → UI).
6. **Define "done"** — format, length, structure, and end with one clear recommendation when comparing.
7. **Let the agent interview you** when requirements are unclear.
8. **Trust but verify** — ask it to break its own work before you review it.
9. **Use XML-style tags** (`<context>`, `<task>`, `<constraints>`) for complex prompts.
10. **Screenshots beat paragraphs** for UI bugs and errors.

## Templates

**New feature**
```
Add <feature> to <project>.
<context>Follow the pattern in <path/to/similar-feature>.</context>
<constraints>
- No new dependencies
- Handle loading, empty, and error states
</constraints>
When done: try to break it, list edge cases, fix what you find.
```

**Interview**
```
I want to <goal>. Don't wait for a spec from me — ask the most important
questions ONE AT A TIME and wait for each answer.
Then summarize the requirements before writing any code.
```

**Bug**
```
Fix: <what happens> when <condition>.
Expected: <what should happen>.
Error/screenshot: <paste>
Code: <paste>
Find the root cause first — don't patch the symptom.
```

**Scoped analysis (saves tokens)**
```
Analyze the <module> only. Identify architecture violations and potential bugs.
Do not modify any files. Focus on the relevant files and their dependencies.
```

**Verify after implementing**
```
Now try to BREAK what you just wrote:
1. Which inputs cause crashes?
2. What if the API returns unexpected data?
3. What if the network times out mid-request?
Fix every issue you find.
```
