#!/usr/bin/env bash
# Run once per clone: points git at the shared hooks folder.
set -e
chmod +x scripts/git-hooks/*
git config core.hooksPath scripts/git-hooks
echo "✔ Git hooks installed (core.hooksPath = scripts/git-hooks)"
