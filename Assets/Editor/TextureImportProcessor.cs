using UnityEngine;
using UnityEditor;

public class TextureImportProcessor : AssetPostprocessor
{
  void OnPreprocessTexture()
  {
    // Chỉ áp dụng cho file mới import, không áp dụng cho file đã có
    if (assetImporter.importSettingsMissing)
    {
      TextureImporter textureImporter = (TextureImporter)assetImporter;

      // Set texture type to Sprite
      textureImporter.textureType = TextureImporterType.Sprite;

      // Set sprite mode to Single
      textureImporter.spriteImportMode = SpriteImportMode.Single;
    }
  }
}
