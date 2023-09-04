# Depra.Assets

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Table of Contents</summary>

- [Introduction](#-introduction)
    - [Features](#-features)
    - [Capabilities](#-capabilities)
- [Installation](#-installation)
- [Contents](#-contents)
- [Usage Examples](#-usage-examples)
    - [Loading an Asset from Resources](#loading-an-asset-from-resources)
    - [Loading an AssetBundle](#loading-an-assetbundle)
    - [Loading an Asset from an AssetBundle](#loading-an-asset-from-an-assetbundle)
    - [Loading an Asset from the Editor Database](#loading-an-asset-from-the-editor-database)
    - [Loading an Asset from Project Settings](#loading-an-asset-from-project-settings)
- [Dependencies](#-dependencies)
- [Collaboration](#-collaboration)
- [Support](#-support)
- [License](#-license)

</details>

## 🧾 Introduction

This library provides classes and interfaces for convenient and efficient loading of various types of assets in
**Unity** projects.

It contains common methods and functionality for working with assets, as well as implementations of specific loading
strategies for different sources.

### 💡 Features:

- **Standardization**: A unified **API** for asset loading from various sources.
- **Cancellation Support**: The ability to cancel loading operations at any point.
- **Progress Tracking**: Providing information on the current loading progress.
- **Extensibility**: A flexible architecture for extending functionality according to your needs.

These features make the library even more powerful and convenient for your tasks.

### 🦾 Capabilities:

| Capability                                         | Runtime | Editor |
|----------------------------------------------------|---------|--------|
| Loading assets from **Resources**                  | ✅       | ✅      |
| Loading **UnityEngine.AssetBundle**                | ✅       | ✅      |
| Loading assets from **UnityEngine.AssetBundle**    | ✅       | ✅      |
| Loading assets from **UnityEditor.PlayerSettings** | ❌       | ✅      |
| Loading assets from **UnityEngine.AssetDatabase**  | ❌       | ✅      |

## 📥 Installation

### 📦 Using **UPM**:

1. Open the **Unity Package Manager** window.
2. Click the **+** button in the upper right corner of the window.
3. Select **Add package from git URL...**.
4. Enter the [repository link](https://github.com/Depra-Inc/Assets.Unity.git).
5. Click **Add**.

### ⚙️ Manual:

Add the following line to `Packages/manifest.json` in the `dependencies` section:

```
"com.depra.assets.unity": "https://github.com/Depra-Inc/Assets.Unity.git"
```

## 📖 Contents

**Key Concepts** used in this library are described in the following interfaces:

- `IAssetIdent`: Designed to facilitate resource management in **Unity** projects.
  It provides a simple and standardized way of identifying and managing assets using **URI**
  *(Uniform Resource Identifier)*.

- `ILoadableAsset<TAsset>`: Defines the fundamental methods and properties required for loading and unloading assets.
  It extends the functionality of the `IAssetFile` interface presented
  in [Depra.Assets](https://github.com/Depra-Inc/Assets) and offers the ability to perform both synchronous and
  asynchronous asset loading, as well as checking the loading state.

You can create your own implementations of these interfaces or use ready-made ones presented in the table:

| Asset class type               | Ident                | Description                                                                                                                                 |
|--------------------------------|----------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| `ResourceAsset<TAsset>`        | `ResourcesPath`      | Loading and unloading assets from `UnityEngine.Resources`.                                                                                  |
| `AssetBundleFile`              | `AssetBundleIdent`   | Loading and unloading `UnityEngine.AssetBundle`.                                                                                            |
| `AssetBundleAssetFile<TAsset>` | `AssetName`          | Loading and unloading assets from `UnityEngine.AssetBundle`.                                                                                |
| `DatabaseAsset<TAsset>`        | `DatabaseAssetIdent` | Loading and unloading assets from the editor's asset database `UnityEditor.AssetDatabase`. ⚠️**Asynchronous loading is not yet supported.** |
| `PreloadedAsset<TAsset>`       | `IAssetIdent`        | Loading and unloading assets from project settings `UnityEditor.ProjectSettings`.                                                           |

All classes implementing the `ILoadableAsset` interface also implement the `System.IDisposable` interface for
convenient usage in `using` blocks.

## 📋 Usage Examples

#### Loading an Asset from Resources

```csharp
var resourceTexture = new ResourceAsset<Texture2D>("Textures/myTexture");
Texture2D loadedTexture = resourceTexture.Load();
// Use the loaded asset.
resourceTexture.Unload();
```

#### Loading an AssetBundle

```csharp
var assetBundleFile = new AssetBundleFile("Path/To/MyBundle");
AssetBundle loadedBundle = assetBundleFile.Load();
// Use the loaded asset.
assetBundleFile.Unload();
```

#### Loading an Asset from an AssetBundle

```csharp
var assetBundle = AssetBundle.LoadFromFile("Path/To/MyBundle");
var assetBundleAsset = new AssetBundleAssetFile<GameObject>("MyAsset", assetBundle);
GameObject loadedAsset = assetBundleAsset.Load();
// Use the loaded asset.
assetBundleAsset.Dispose();
```

#### Loading an Asset from the Editor Database

```csharp
var databaseAsset = new DatabaseAsset<MyScriptableObject>("Path/To/MyAsset");
MyScriptableObject loadedObject = databaseAsset.Load();
// Use the loaded asset.
databaseAsset.Dispose();
```

#### Loading an Asset from Project Settings

```csharp
var preloadedAsset = new PreloadedAsset<GameObject>("Path/To/MyAsset");
GameObject loadedAsset = preloadedAsset.Load();
// Use the loaded asset.
preloadedAsset.Dispose();
```

## 🖇 Dependencies

- [Depra.Assets](https://github.com/Depra-Inc/Assets.git) - the base library for working with assets (provided
  with this **UPM** package).

## 🤝 Collaboration

I welcome feature requests and bug reports in
the [issues section](https://github.com/Depra-Inc/Assets.Unity/issues), and I also
accept [pull requests](https://github.com/Depra-Inc/Assets.Unity/pulls).

## 🫂 Support

I am an independent developer, and most of the development of this project is done in my free time. If you are
interested in collaborating or hiring me for a project, please check out
my [portfolio](https://github.com/Depra-Inc) and [contact me](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 License

This project is distributed under the
**[Apache-2.0 license](https://github.com/Depra-Inc/Assets.Unity/blob/main/LICENSE.md)**

Copyright (c) 2023 Nikolay Melnikov
[n.melnikov@depra.org](mailto:n.melnikov@depra.org)