# Git hooks

Shared hooks for this repo. Enable them **once per clone**:

```sh
git config core.hooksPath .githooks
```

## `pre-commit`

Keeps the agent skills in sync and staged on every commit:

- `.agents/skills/` — canonical store managed by `npx skills`
- `.claude/skills/` — portable real copies that Claude Code reads (no symlinks, Windows-safe)

After `npx skills add` / `npx skills update`, just commit — the hook mirrors and stages both trees.

## Installing / using the skills as a teammate

Nothing to install for Claude Code: `.claude/skills/` is committed, so after `git pull` the
skills load automatically. `skills-lock.json` pins versions for `npx skills` if you prefer that route.
