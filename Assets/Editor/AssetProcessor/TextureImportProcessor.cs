using System;
using UnityEditor;
using UnityEngine;

public class TextureImportProcessor : AssetPostprocessor
{
    private const string TextureFolder = "Assets/Textures/";
    private const string UIFolder = "Assets/UI/";
    private const int MaxSpriteSize = 2048;
    private const int MaxUISpriteSize = 1024;

    void OnPreprocessTexture()
    {
        if (assetImporter is not TextureImporter importer)
            return;

        string path = assetPath.Replace('\\', '/');

        // 规则1：UI 文件夹
        if (path.StartsWith(UIFolder, StringComparison.Ordinal))
        {
            ApplyUIPreset(importer);
            Debug.Log($"[TextureImport - UI] 已自动处理: {assetPath}");
            return;
        }

        // 规则2：Textures 文件夹
        if (path.StartsWith(TextureFolder, StringComparison.Ordinal))
        {
            ApplySpritePreset(importer);
            Debug.Log($"[TextureImport - Sprite] 已自动处理: {assetPath}");
            return;
        }
    }

    private static void ApplySpritePreset(TextureImporter importer)
    {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.maxTextureSize = MaxSpriteSize;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.isReadable = false;
        importer.mipmapEnabled = false;
    }

    private static void ApplyUIPreset(TextureImporter importer)
    {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.maxTextureSize = MaxUISpriteSize;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.isReadable = true;
        importer.mipmapEnabled = false;
    }
}