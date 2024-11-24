using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using UnityEditor.AssetImporters;

using TargetFile = Translations.Mapping.Mapping;

namespace Translations.Editor.Mapping
{
    [ScriptedImporter(VERSION, TargetFile.EXTENSION)]
    internal class MappingImporter : ScriptedImporter
    {
        private const int VERSION = 0;
        private const string DEFAULT_ASSET_CONTENT = "";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            string text;
            try
            {
                text = File.ReadAllText(ctx.assetPath);
            }
            catch (Exception e)
            {
                ctx.LogImportError($"Could not read file `{ctx.assetPath}` ({e})");
                return;
            }

            var asset = ScriptableObject.CreateInstance<TargetFile>();

            try
            {
                JsonUtility.FromJsonOverwrite(text, asset);
            }
            catch (Exception e)
            {
                ctx.LogImportError($"Could not parse asset in JSON format from '{ctx.assetPath}' ({e})");
                DestroyImmediate(asset);
                return;
            }

            asset.name = Path.GetFileNameWithoutExtension(assetPath);
            ctx.AddObjectToAsset("<root>", asset);
        }

        [MenuItem("Assets/Create/Translations/Translation Mapping")]
        public static void CreateAsset()
        {
            ProjectWindowUtil.CreateAssetWithContent($"Translation Mapping.{TargetFile.EXTENSION}",
                DEFAULT_ASSET_CONTENT);
        }
    }
}