## 🔧 NuVerNet - .NET Project Versioning & Packaging CLI Tool

**NuVerNet** is a powerful yet lightweight CLI tool designed for .NET developers managing large solutions with multiple interdependent projects.

It automates version bumping, NuGet packaging, and publishing workflows — all while respecting your project dependency hierarchy.

---

### ✨ Key Features

- ✅ **Smart Version Bumping**  
  Easily bump versions of a selected project (`patch`, `minor`, `major`) and propagate the change to dependent projects.

- 📦 **Dependency-Aware NuGet Packaging**  
  Automatically packs projects in topological order based on their dependencies.

- 🚀 **Seamless NuGet Push**  
  Push generated packages to your NuGet server in the correct dependency order.

- 🔍 **Dependency Graph Traversal**  
  Ensures correct update flow through your solution tree.

---

### 💻 Example Commands

```bash
nuvernet bump --project ./MyLib/MyLib.csproj --level minor
nuvernet pack --project ./MyLib/MyLib.csproj
nuvernet push --source nuget
```

---

### 🧹 Ideal For

- Enterprise solutions with dozens of interdependent projects
- Teams that publish internal NuGet packages
- Developers who want reproducible, version-safe packaging
