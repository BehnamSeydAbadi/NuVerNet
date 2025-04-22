# NuVerNet

**NuVerNet** is a .NET versioning automation tool that simplifies the process of versioning, packaging, and publishing NuGet packages.

---

## 🚀 Overview

NuVerNet automates several tedious tasks in the NuGet package management workflow:

- ✅ Version bumping (`major`, `minor`, `patch`)
- 📦 Project packaging into NuGet packages
- ☁️ Publishing packages to NuGet servers

---

## 📦 Installation

You can install NuVerNet locally without relying on the NuGet server:

1. **Build the project in Release mode or download exe from release page**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Add the executable to your system PATH**:
   - Copy the path to the `publish` folder
   - Add it to your system's environment variables under `PATH`

3. **Run the tool from anywhere using**:
   ```bash
   nuvernet bump --help
   ```

---

## 🛠 Usage

The primary command in NuVerNet is `bump`, which handles the entire versioning and publishing workflow:

```bash
nuvernet bump --csprojpath <path-to-csproj> \
           --solutionpath <path-to-solution> \
           --outputpath <output-directory> \
           --nugetserverurl <nuget-server-url> \
           [--level <version-level>]
```

---

## 📌 Parameters

| Parameter          | Description                                   | Required |
|-------------------|-----------------------------------------------|----------|
| `--csprojpath`     | Path to the main `.csproj` file                | ✅ Yes |
| `--solutionpath`   | Path to the `.sln` file                        | ✅ Yes |
| `--outputpath`     | Directory where `.nupkg` files will be saved  | ✅ Yes |
| `--nugetserverurl` | URL of the NuGet server for publishing        | ✅ Yes |
| `--level`          | Version level to bump (`major`, `minor`, `patch`) | ❌ Optional (defaults to `patch`) |

---

## 💡 Examples

### Bump the patch version and publish:

```bash
nuvernet bump --csprojpath ./src/MyProject/MyProject.csproj \
           --solutionpath ./MySolution.sln \
           --outputpath ./nuget-packages \
           --nugetserverurl https://api.nuget.org/v3/index.json
```

### Bump the minor version instead:

```bash
nuvernet bump --csprojpath ./src/MyProject/MyProject.csproj \
           --solutionpath ./MySolution.sln \
           --outputpath ./nuget-packages \
           --nugetserverurl https://api.nuget.org/v3/index.json \
           --level minor
```

---

## 🔄 Workflow

When you run the `bump` command, NuVerNet performs the following steps:

1. Loads all project files from the specified solution
2. Bumps the version numbers according to the specified level
3. Writes the updated version information back to the project files
4. Packages the projects into `.nupkg` files
5. Pushes the packages to the specified NuGet server

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
