# Depra.Assets

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Table of Contents</summary>

- [Introduction](#-introduction)
    - [Features](#-features)
    - [Capabilities](#-capabilities)
- [Getting_Started](#-getting-started)
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

This library provides classes and interfaces for convenient and efficient loading of various types of resources in 
**Unity** projects.

It contains common methods and functionality for working with assets, as well as implementations of specific loading
strategies for different sources.

### 💡 Features:

- A uniform **API** for loading assets from different sources.
- Support for canceling loading operations.
- Providing information about the loading progress.
- Extensibility.

### 🦾 Capabilities:

| Capability                                         | Runtime | Editor |
|----------------------------------------------------|---------|--------|
| Loading assets from **Resources**                  | ✅       | ✅      |
| Loading **UnityEngine.AssetBundle**                | ✅       | ✅      |
| Loading assets from **UnityEngine.AssetBundle**    | ✅       | ✅      |
| Loading assets from **UnityEditor.PlayerSettings** | ❌       | ✅      |
| Loading assets from **UnityEngine.AssetDatabase**  | ❌       | ✅      |

## 🚀 Getting Started

Before you begin using the **Depra.Assets** library in your **Unity** project,
make sure your project meets the following conditions:

### Install UniTask

**Depra.Assets** uses the **UniTask** library for asynchronous operations.
You can install it by following [these instructions](https://github.com/Cysharp/UniTask#getting-started).

## 📥 Installation

### 📦 Using **UPM**:

1. Open the **Unity Package Manager** window.
2. Click the **+** button in the upper right corner of the window.
3. Select **Add package from git URL...**.
4. Enter the [repository link](https://github.com/Depression-aggression/Assets.Unity.git).
5. Click **Add**.

### ⚙️ Manual:

Add the following line to `Packages/manifest.json` in the `dependencies` section:

```json
"com.depra.assets.unity": "https://github.com/Depression-aggression/Assets.Unity.git"
```

## 📖 Contents

- `IUnityLoadableAsset<TAsset>` - defines basic methods and properties for loading and unloading assets. It extends
  the `IAssetFile` interface from [Depra.Assets](https://github.com/Depression-aggression/Assets) and provides
  capabilities for synchronous and asynchronous loading, as well as checking the loading state.

```csharp
public interface IUnityLoadableAsset<TAsset> : IAssetFile
{
    bool IsLoaded { get; }
    
    TAsset Load();
    
    void Unload();
    
    UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
        CancellationToken cancellationToken = default);
}
```

- `ResourceAsset<TAsset>` - provides an implementation for loading and unloading assets from **Unity** resources.

- `AssetBundleFile` - provides methods for loading and unloading `UnityEngine.AssetBundle`.

- `AssetBundleAssetFile<TAsset>` - ensures loading and unloading of assets from `UnityEngine.AssetBundle`.

- `DatabaseAsset<TAsset>` - allows loading and unloading assets from the editor's asset database
  `UnityEditor.AssetDatabase`.

- `PreloadedAsset<TAsset>` - provides loading and unloading of assets from project
  settings `UnityEditor.ProjectSettings`.

All classes implementing the `IUnityLoadableAsset` interface also implement the `System.IDisposable` interface for
convenient usage in `using` blocks.

## 📋 Usage Examples

### Loading an Asset from Resources

```csharp
var resourceTexture = new ResourceAsset<Texture2D>("Textures/myTexture");
Texture2D loadedTexture = resourceTexture.Load();
// Use the loaded asset.
resourceTexture.Unload();
```

### Loading an AssetBundle

```csharp
var assetBundleFile = new AssetBundleFile("Path/To/MyBundle");
AssetBundle loadedBundle = assetBundleFile.Load();
// Use the loaded asset.
assetBundleFile.Unload();
```

### Loading an Asset from an AssetBundle

```csharp
var assetBundle = AssetBundle.LoadFromFile("Path/To/MyBundle");
var assetBundleAsset = new AssetBundleAssetFile<GameObject>("MyAsset", assetBundle);
GameObject loadedAsset = assetBundleAsset.Load();
// Use the loaded asset.
assetBundleAsset.Dispose();
```

### Loading an Asset from the Editor Database

```csharp
var databaseAsset = new DatabaseAsset<MyScriptableObject>("Path/To/MyAsset");
MyScriptableObject loadedObject = databaseAsset.Load();
// Use the loaded asset.
databaseAsset.Dispose();
```

### Loading an Asset from Project Settings

```csharp
var preloadedAsset = new PreloadedAsset<GameObject>("Path/To/MyAsset");
GameObject loadedAsset = preloadedAsset.Load();
// Use the loaded asset.
preloadedAsset.Dispose();
```

## 🖇 Dependencies

- [Depra.Assets](https://github.com/Depression-aggression/Assets) - the base library for working with assets (provided
  with this UPM package).
- [UniTask](https://github.com/Cysharp/UniTask) - a library for asynchronous operations.

## 🤝 Collaboration

I welcome feature requests and bug reports in
the [issues section](https://github.com/Depression-aggression/Assets.Unity/issues), and I also
accept [pull requests](https://github.com/Depression-aggression/Assets.Unity/pulls).

## 🫂 Support

I am an independent developer, and most of the development of this project is done in my free time. If you are
interested in collaborating or hiring me for a project, please check out
my [portfolio](https://github.com/Depression-aggression) and [contact me](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 License

This project is distributed under the
**[Apache-2.0 license](https://github.com/Depression-aggression/Assets.Unity/blob/main/LICENSE.md)**

Copyright (c) 2023 Nikolay Melnikov
[g0dzZz1lla@yandex.ru](mailto:g0dzZz1lla@yandex.ru)