import fs from "fs/promises";
import path from "path";

const ROOT = process.cwd();

export async function generateClaudeMd() {
  console.log("🔍 Analyzing repository...");

  const repoInfo = await analyzeRepository(ROOT);

  console.log("📝 Generating CLAUDE.md...");

  const claudeMd = buildClaudeMd(repoInfo);

  await fs.writeFile(path.join(ROOT, "CLAUDE.md"), claudeMd, "utf8");

  console.log("✅ CLAUDE.md generated");
}

async function analyzeRepository(rootDir) {
  const info = {
    projectName: path.basename(rootDir),
    techStack: [],
    commands: {},
    folders: [],
    conventions: [],
    workflows: [],
  };

  // Detect package.json
  const packageJsonPath = path.join(rootDir, "package.json");

  if (await exists(packageJsonPath)) {
    const packageJson = JSON.parse(await fs.readFile(packageJsonPath, "utf8"));

    info.techStack.push("Node.js");

    if (packageJson.dependencies?.next) {
      info.techStack.push("Next.js");
    }

    if (packageJson.dependencies?.react) {
      info.techStack.push("React");
    }

    info.commands = packageJson.scripts || {};
  }

  // Detect .NET
  const files = await fs.readdir(rootDir);

  if (files.some((f) => f.endsWith(".sln"))) {
    info.techStack.push(".NET");
  }

  // Detect folders
  info.folders = await getTopLevelFolders(rootDir);

  // Detect conventions
  info.conventions = detectConventions(info);

  return info;
}

function buildClaudeMd(info) {
  return `# ${info.projectName}

## Project Overview

Repository documentation generated automatically.

---

## Tech Stack

${info.techStack.map((t) => `- ${t}`).join("\n")}

---

## Repository Structure

${info.folders.map((f) => `- \`${f}\``).join("\n")}

---

## Development Commands

${Object.entries(info.commands)
  .map(([key, value]) => `- \`${key}\` → \`${value}\``)
  .join("\n")}

---

## Coding Conventions

${info.conventions.map((c) => `- ${c}`).join("\n")}

---

## Workflow Rules

- Follow existing repository patterns
- Do not introduce new architectural styles without consistency
- Prefer extending existing modules over duplication

---

## AI Agent Guidance

### Read First
- README.md
- package.json
- tsconfig.json
- CI/CD configuration
- Environment configuration

### Important Rules
- Do not hallucinate commands
- Preserve existing patterns
- Keep changes minimal and focused

---

## Documentation Index

- ./docs/architecture.md
- ./docs/conventions.md
- ./docs/testing.md
- ./docs/deployment.md
- ./docs/conventions.md
`;

}

function detectConventions(info) {
  const conventions = [];

  if (info.techStack.includes("React")) {
    conventions.push("Component-based frontend architecture");
  }

  if (info.techStack.includes(".NET")) {
    conventions.push("Layered .NET application structure");
    conventions.push("Dependency injection via built-in container");
    conventions.push("Async/await patterns for IO operations");
  }

  return conventions;
}

async function getTopLevelFolders(rootDir) {
  const entries = await fs.readdir(rootDir, {
    withFileTypes: true,
  });

  return entries
    .filter((e) => e.isDirectory())
    .map((e) => e.name)
    .filter((name) => !name.startsWith("."));
}

async function exists(filePath) {
  try {
    await fs.access(filePath);
    return true;
  } catch {
    return false;
  }
}

// Run directly
if (import.meta.url === `file://${process.argv[1]}`) {
  generateClaudeMd().catch((err) => {
    console.error("❌ Failed:", err);
    process.exit(1);
  });
}
