---
name: generate-claude-md
description: Generates or updates CLAUDE.md files with dotnet project-specific configurations and instructions for Claude Code.
---

# CLAUDE.md Generator

This skill helps generate or update CLAUDE.md files with dotnet project-specific configurations and instructions for Claude Code.

# CLAUDE.md Generator Skill

## Purpose

This skill analyzes a repository and generates or updates a high-quality `CLAUDE.md` file optimized for:
- AI coding agents
- Developer onboarding
- Repository navigation
- Workflow consistency

The generated documentation should be:
- concise
- repository-specific
- evidence-based
- maintainable
- progressively disclosed

---

# Core Principles

## 1. Never Hallucinate

Only document:
- commands that exist
- dependencies that are installed
- workflows observable in code/configuration
- conventions actually used in the repository

If something is uncertain:
- explicitly mark it as inferred
- avoid presenting guesses as facts

---

## 2. Prefer Progressive Disclosure

Keep `CLAUDE.md` lightweight.

Move detailed documentation into:
- `/docs/architecture.md`
- `/docs/development.md`
- `/docs/testing.md`
- `/docs/deployment.md`
- `/docs/conventions.md`

Then link those docs from `CLAUDE.md`.

---

## 3. Optimize for AI Agents

Structure documentation for fast scanning:
- bullet lists
- short sections
- concrete examples
- predictable headings

Avoid:
- marketing language
- long prose
- vague explanations

---

# Repository Analysis Workflow

Before generating documentation:

1. Inspect repository structure
2. Detect languages/frameworks
3. Read package manifests
4. Read Docker/CI configs
5. Inspect build scripts
6. Inspect test setup
7. Detect architectural patterns
8. Detect coding conventions
9. Detect environment/config strategy
10. Detect deployment workflow

Important files to inspect:
- package.json
- pnpm-workspace.yaml
- turbo.json
- nx.json
- tsconfig.json
- vite.config.*
- next.config.*
- Dockerfile
- docker-compose.*
- .github/workflows/*
- README.md
- Makefile
- *.sln
- *.csproj
- Program.cs
- appsettings*.json
- requirements.txt
- pyproject.toml
- go.mod
- Cargo.toml

---

# Expected Output Structure

## CLAUDE.md

Should contain:

- Project Overview
- Tech Stack
- Repository Structure
- Development Commands
- Environment & Configuration
- Coding Conventions
- Workflow Rules
- AI Agent Guidance
- Documentation Index

Keep concise.

---

# Coding Convention Detection

Infer conventions from:
- folder naming
- dependency injection patterns
- logging style
- error handling
- DTO/entity naming
- async usage
- API routing
- frontend component structure
- testing libraries
- formatting configs

Never invent conventions.

---

# Workflow Rules

Document:
- where new code belongs
- forbidden patterns
- migration process
- testing expectations
- PR expectations
- deployment precautions
- security-sensitive areas

---

# AI Agent Guidance

Include:
- safest extension points
- dangerous areas
- generated code locations
- common pitfalls
- files to read first
- patterns to replicate

---

# File Generation Rules

## Main File
Generate:
- `/CLAUDE.md`

## Detailed Docs
Generate only if needed:
- `/docs/architecture.md`
- `/docs/testing.md`
- `/docs/deployment.md`
- `/docs/conventions.md`

Do not create unnecessary docs.

---

# Update Strategy

When `CLAUDE.md` already exists:
- preserve useful custom sections
- update stale information
- avoid destructive overwrites

Prefer idempotent updates.

---

# Output Quality Checklist

Before finishing:

- Commands verified
- No hallucinated tools/frameworks
- Repo structure explained
- Architecture summarized
- Conventions documented
- Workflow rules actionable
- AI guidance included
- Progressive disclosure applied
- Concise and scannable

---

# Preferred Tone

- technical
- direct
- precise
- low-noise
- implementation-focused
