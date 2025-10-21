#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ObscuraX.Unity.Editor
{
    [CreateAssetMenu(fileName = "ObscuraXConfig", menuName = "ObscuraX/Create ObscuraX Configuration")]
    public class ObscuraXConfig : ScriptableObject
    {
        [Header("ObscuraX Obfuscation")]
        [Tooltip("Enable ObscuraX obfuscation during Unity builds")]
        public bool EnableObfuscation = true;

        [Header("Configuration Path")]
        [Tooltip("Custom path to ObscuraX configuration files (leave empty for auto-detection)")]
        public string ConfigPath = "";

        public string DetectedConfigPath => GetDetectedConfigPath();

        [Header("Protection Settings")]
        [Tooltip("Use Unity UI to edit protections (false = use protections.json file directly)")]
        public bool UseUnityUIForProtections = true;

        [Tooltip("Protection settings loaded from protections.json")]
        public List<ProtectionSetting> ProtectionSettings = new List<ProtectionSetting>();

        [Header("Obfuscation Settings")]
        [Tooltip("Timeout for obfuscation process in minutes (default: 5 minutes)")]
        public int ObfuscationTimeoutMinutes = 5;

        [Tooltip("Enable debug logging for detailed ObscuraX output (useful for troubleshooting)")]
        public bool EnableDebugLogging = false;

        public string GetDetectedConfigPath()
        {
            if (!string.IsNullOrEmpty(ConfigPath) && Directory.Exists(ConfigPath))
            {
                return ConfigPath;
            }

            var bitMonoCli = FindObscuraXCli();
            if (!string.IsNullOrEmpty(bitMonoCli))
            {
                var cliDir = Path.GetDirectoryName(bitMonoCli);
                if (File.Exists(Path.Combine(cliDir, "obfuscation.json")))
                {
                    return cliDir;
                }
            }

            return "Not found - ObscuraX.CLI or config files missing";
        }

        private string FindObscuraXCli()
        {
            var paths = new[]
            {
                // Inside package under Assets
                Path.Combine(Application.dataPath, "ObscuraX.Unity", "ObscuraX.CLI", "ObscuraX.CLI.exe"),
                Path.Combine(Application.dataPath, "ObscuraX.CLI", "ObscuraX.CLI.exe"),
                // Project root sibling to Assets (local dev / CI setups)
                Path.Combine(Application.dataPath, "..", "ObscuraX.CLI", "ObscuraX.CLI.exe"),
                Path.Combine(Application.dataPath, "..", "..", "src", "ObscuraX.CLI", "bin", "Release", "net462", "ObscuraX.CLI.exe"),
                "ObscuraX.CLI.exe"
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        public void LoadProtectionsFromFile()
        {
            var configPath = GetDetectedConfigPath();
            if (string.IsNullOrEmpty(configPath) || configPath.Contains("Not found"))
            {
                ProtectionSettings.Clear();
                return;
            }

            var protectionsFile = Path.Combine(configPath, "protections.json");
            if (!File.Exists(protectionsFile))
            {
                ProtectionSettings.Clear();
                return;
            }

            try
            {
                var json = File.ReadAllText(protectionsFile);
                var protectionsData = JsonUtility.FromJson<ProtectionsData>(json);

                ProtectionSettings.Clear();
                if (protectionsData?.Protections != null && protectionsData.Protections.Length > 0)
                {
                    ProtectionSettings.AddRange(protectionsData.Protections);
                }

                UnityEditor.EditorUtility.SetDirty(this);
            }
            catch (Exception)
            {
                ProtectionSettings.Clear();
            }
        }

        public void SaveProtectionsToFile()
        {
            var configPath = GetDetectedConfigPath();
            if (string.IsNullOrEmpty(configPath) || configPath.Contains("Not found"))
                return;

            var protectionsFile = Path.Combine(configPath, "protections.json");

            try
            {
                var protectionsData = new ProtectionsData { Protections = ProtectionSettings.ToArray() };
                var json = JsonUtility.ToJson(protectionsData, true);
                File.WriteAllText(protectionsFile, json);
            }
            catch (Exception)
            {
            }
        }
    }

    [Serializable]
    public class ProtectionSetting
    {
        public string Name;
        public bool Enabled;
    }

    [Serializable]
    public class ProtectionsData
    {
        public ProtectionSetting[] Protections;
    }
}
#endif