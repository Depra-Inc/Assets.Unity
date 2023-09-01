# Depra.Assets

<div align="center">
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Оглавление</summary>

- [Введение](#введение)
    - [Особенности](#особенности)
- [Установка](#установка)
- [Содержание](#содержание)
- [Примеры использования](#примеры-использования)
- [Поддержка](#поддержка)
- [Лицензия](#лицензия)

</details>

## Введение

Эта библиотека предоставляет классы и интерфейсы для удобной и эффективной загрузки различных типов ресурсов в **Unity**
проектах.

Она содержит общие методы и функциональность для работы с ассетами, а также реализации специфичных стратегий
загрузки для различных источников.

### Особенности:

- Однородный **API** для загрузки ассетов из различных источников;
- Поддержка отмены загрузки;
- Предоставление информации о прогрессе загрузки;
- Расширяемость.

### Возможности:

| Возможность                                        | Runtime | Редакторе |
|----------------------------------------------------|---------|-----------|
| Загрузка ассетов из **Resources**                  | ✅       | ✅         |
| Загрузка **UnityEngine.AssetBundle**               | ✅       | ✅         |
| Загрузка ассетов из **UnityEngine.AssetBundle**    | ✅       | ✅         |
| Загрузка ассетов из **UnityEditor.PlayerSettings** | ❌       | ✅         |
| Загрузка ассетов из **UnityEngine.AssetDatabase**  | ❌       | ✅         |

## Установка

### Через **UPM**:

1. Откройте окно **Unity Package Manager**.
2. Нажмите на кнопку **+** в правом верхнем углу окна.
3. Выберите **Add package from git URL...**.
4. Введите [ссылку на репозиторий](https://github.com/Depression-aggression/Assets.Unity.git)
5. Нажмите **Add**.

### Ручная:

Добавьте в `Packages/manifest.json` в раздел `dependencies` следующую строку:

```json
"com.depra.assets.unity": "https://github.com/Depression-aggression/Assets.Unity.git"
```

## Содержание

- `IUnityLoadableAsset<TAsset>` - определяет базовые методы и свойства для загрузки и выгрузки ассетов.
  Он расширяет интерфейс `IAssetFile` из [Depra.Assets](https://github.com/Depression-aggression/Assets) и
  предоставляет возможности для синхронной и асинхронной загрузки, а также проверки состояния загрузки.

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

- `ResourceAsset<TAsset>` - предоставляет реализацию загрузки и выгрузки ассетов из ресурсов **Unity**.


- `AssetBundleFile` - предоставляет методы для загрузки и выгрузки `UnityEngine.AssetBundle`.


- `AssetBundleAssetFile<TAsset>` - обеспечивает загрузку и выгрузку ассетов из `UnityEngine.AssetBundle`.


- `DatabaseAsset<TAsset>` - позволяет загружать и выгружать ассеты из редакторской базы
  данных `UnityEditor.AssetDatabase`.


- `PreloadedAsset<TAsset>` - обеспечивает загрузку и выгрузку ассетов из настроек проекта `UnityEditor.ProjectSettings`.

Все классы, реализующие интерфейс `IUnityLoadableAsset`, также реализуют интерфейс `System.IDisposable`.
Добавлено для удобного использования в `using` блоках.

## Примеры использования

### Загрузка ассета из ресурсов

```csharp
var resourceIdent = new ResourcePath("Textures/myTexture");
var resourceTexture = new ResourceAsset<Texture2D>(resourceIdent);
Texture2D loadedTexture = resourceTexture.Load();
// Использование загруженного ассета.
resourceTexture.Unload();
```

### Загрузка AssetBundle

```csharp
var assetBundleIdent = new AssetBundleIdent("Path/To/MyBundle");
var assetBundleFile = new AssetBundleFile(assetBundleIdent);
AssetBundle loadedBundle = assetBundleFile.Load();
// Использование загруженного ассета.
assetBundleFile.Unload();
```

### Загрузка ассета из AssetBundle

```csharp
var assetName = new AssetName("MyAsset");
var assetBundle = AssetBundle.LoadFromFile("Path/To/MyBundle");
var assetBundleAsset = new AssetBundleAssetFile<GameObject>(assetName, assetBundle);
GameObject loadedAsset = assetBundleAsset.Load();
// Использование загруженного ассета.
assetBundleAsset.Dispose();
```

### Загрузка ассета из редакторской базы данных

```csharp
var databaseAssetIdent = new DatabaseAssetIdent("relativeDirectory", "MyDatabaseAsset", ".asset");
var databaseAsset = new DatabaseAsset<MyScriptableObject>(databaseAssetIdent);
MyScriptableObject loadedObject = databaseAsset.Load();
// Использование загруженного ассета.
databaseAsset.Dispose();
```

### Загрузка ассета из настроек проекта

```csharp
var preloadedAsset = new PreloadedAsset<GameObject>("MyPreloadedObject");
GameObject loadedAsset = preloadedAsset.Load();
// Использование загруженного ассета.
preloadedAsset.Dispose();
```

## Зависимости

- [Depra.Assets](https://github.com/Depression-aggression/Assets) - базовая библиотека для работы с ассетами.
- [UniTask](https://github.com/Cysharp/UniTask) - библиотека для работы с асинхронными операциями.

## Поддержка

Я независимый разработчик,
и большая часть разработки этого проекта выполняется в свободное время.
Если вы заинтересованы в сотрудничестве или найме меня для проекта,
ознакомьтесь с моим [портфолио](https://github.com/Depression-aggression)
и [свяжитесь со мной](mailto:g0dzZz1lla@yandex.ru)!

## Лицензия

Этот проект распространяется под лицензией **Apache-2.0**

Copyright (c) 2023 Николай Мельников
[g0dzZz1lla@yandex.ru](mailto:g0dzZz1lla@yandex.ru)