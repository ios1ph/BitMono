#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ObscuraX.Editor
{
    public static class PackageExporter
    {
        public static void ExportPackage()
        {
            try
            {
                var version = Environment.GetEnvironmentVariable("VERSION") ?? Environment.GetEnvironmentVariable("PACKAGE_VERSION");
                var unityVersion = Environment.GetEnvironmentVariable("UNITY_VERSION");

                if (string.IsNullOrWhiteSpace(version))
                {
                    Debug.LogError("VERSION/PACKAGE_VERSION environment variable is missing or empty. Aborting export.");
                    EditorApplication.Exit(1);
                    return;
                }

                if (string.IsNullOrWhiteSpace(unityVersion))
                {
                    Debug.LogError("UNITY_VERSION environment variable is missing or empty. Aborting export.");
                    EditorApplication.Exit(1);
                    return;
                }
                
                var outputPath = Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    "..",
                    $"ObscuraX-Unity-v{version}-Unity{unityVersion}.unitypackage"
                );
                
                const string basePath = "Assets/ObscuraX.Unity";
                
                var coreFiles = new[]
                {
                    "Editor/ObscuraXBuildProcessor.cs",
                    "Editor/ObscuraXConfig.cs",
                    "Editor/ObscuraXConfigInspector.cs",
                    "Editor/ObscuraX.Unity.Editor.asmdef",
                    "package.json",
                    "ObscuraXConfig.asset"
                };
                
                List<string> assetsToInclude = new List<string>();
                foreach (var file in coreFiles)
                {
                    var assetPath = $"{basePath}/{file}";
                    if (File.Exists(assetPath))
                    {
                        assetsToInclude.Add(assetPath);
                        Debug.Log($"Including: {assetPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"File not found (skipping): {assetPath}");
                    }
                }
                
                var cliFolder = Path.Combine(Application.dataPath, "ObscuraX.Unity/ObscuraX.CLI");
                if (Directory.Exists(cliFolder))
                {
                    var cliFiles = Directory.GetFiles(cliFolder, "*", SearchOption.AllDirectories)
                        .Where(f => !f.EndsWith(".meta"))
                        .Select(f => f.Replace(Application.dataPath, "Assets").Replace("\\", "/"))
                        .ToArray();
                    
                    assetsToInclude.AddRange(cliFiles);
                    Debug.Log($"Including {cliFiles.Length} files from ObscuraX.CLI folder");
                }
                else
                {
                    Debug.LogWarning($"ObscuraX.CLI folder not found: {cliFolder}");
                }
                
                if (assetsToInclude.Count == 0)
                {
                    Debug.LogError("No valid assets found to include in package!");
                    EditorApplication.Exit(1);
                    return;
                }
                
                Debug.Log($"Total assets to export: {assetsToInclude.Count}");
                
                AssetDatabase.ExportPackage(
                    assetsToInclude.ToArray(),
                    outputPath,
                    ExportPackageOptions.Default
                );
                if (File.Exists(outputPath))
                {
                    var fileInfo = new FileInfo(outputPath);
                    Debug.Log($"Package exported successfully!");
                    Debug.Log($"  Path: {outputPath}");
                    Debug.Log($"  Size: {fileInfo.Length / 1024.0 / 1024.0:F2} MB");
                    Debug.Log($"  Assets: {assetsToInclude.Count}");
                }
                else
                {
                    Debug.LogError($"Package file was not created: {outputPath}");
                    EditorApplication.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to export package: {ex}");
                EditorApplication.Exit(1);
                throw;
            }
        }
    }
}
#endif
