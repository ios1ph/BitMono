#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ObscuraX.Unity.Editor
{
    public class ObscuraXBuildProcessor : IPostBuildPlayerScriptDLLs
    {
        public int callbackOrder => 0;

        private static CancellationTokenSource _cancellationTokenSource;
        private static Process _currentProcess;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.quitting += OnApplicationQuitting;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (_currentProcess != null && !_currentProcess.HasExited)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    LogDebugStatic("Detected Unity play mode change, cancelling obfuscation");
                    CancelObfuscation();
                }
            }
        }

        private static void OnApplicationQuitting()
        {
            CancelObfuscation();
        }

        public static void CancelObfuscation()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                LogDebugStatic("Cancelling obfuscation process...");
                _cancellationTokenSource.Cancel();
            }

            if (_currentProcess != null && !_currentProcess.HasExited)
            {
                try
                {
                    LogDebugStatic("Killing obfuscation process...");
                    _currentProcess.Kill();
                    _currentProcess.WaitForExit(2000);
                }
                catch (Exception ex)
                {
                    LogDebugStatic($"Error killing process: {ex}");
                }
            }
        }

        [MenuItem("ObscuraX/Show Config Location", false, 1)]
        public static void ShowConfigLocation()
        {
            var configs = AssetDatabase.FindAssets("t:ObscuraXConfig");
            if (configs.Length > 0)
            {
                var configPath = AssetDatabase.GUIDToAssetPath(configs[0]);
                var config = AssetDatabase.LoadAssetAtPath<ObscuraXConfig>(configPath);
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
            else
            {
                EditorUtility.DisplayDialog("ObscuraX Configuration", "No ObscuraX configuration found. Please create a ObscuraXConfig asset.", "OK");
            }
        }

        [MenuItem("ObscuraX/Documentation", false, 2)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/sunnamed434/ObscuraX#readme");
        }

        [MenuItem("ObscuraX/GitHub Repository", false, 3)]
        public static void OpenGitHub()
        {
            Application.OpenURL("https://github.com/sunnamed434/ObscuraX");
        }

        [MenuItem("ObscuraX/Report Issue", false, 4)]
        public static void ReportIssue()
        {
            Application.OpenURL("https://github.com/sunnamed434/ObscuraX/issues");
        }

        [MenuItem("ObscuraX/About ObscuraX", false, 5)]
        public static void ShowAboutDialog()
        {
            var version = "Unknown";
            try
            {
                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.obscurax.unity/package.json");
                if (packageInfo != null)
                {
                    version = packageInfo.version;
                }
                else
                {
                    var guids = AssetDatabase.FindAssets("package t:TextAsset", new[] { "Assets/ObscuraX.Unity" });
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (path.EndsWith("package.json"))
                        {
                            var json = File.ReadAllText(path);
                            var packageData = JsonUtility.FromJson<PackageJson>(json);
                            version = packageData.version ?? "Unknown";
                            break;
                        }
                    }
                }
            }
            catch
            {
                // Fallback to unknown
            }

            EditorUtility.DisplayDialog("About ObscuraX",
                "ObscuraX Unity Integration\n\n" +
                "Professional obfuscation for Unity projects with Mono2x support.\n" +
                "Automatically obfuscates your assemblies during build process.\n\n" +
                $"Version: {version}\n" +
                "Author: sunnamed434\n" +
                "GitHub: https://github.com/sunnamed434/ObscuraX\n\n" +
                "Note: IL2CPP is not supported yet, however is planned to be supported in the future.",
                "OK");
        }

        [System.Serializable]
        private class PackageJson
        {
            public string version;
        }

        private void LogDebug(string message)
        {
            var config = LoadObscuraXConfig();
            if (config != null && config.EnableDebugLogging)
            {
                UnityEngine.Debug.Log($"[ObscuraX] {message}");
            }
        }

        private static void LogDebugStatic(string message)
        {
            var config = LoadObscuraXConfigStatic();
            if (config != null && config.EnableDebugLogging)
            {
                UnityEngine.Debug.Log($"[ObscuraX] {message}");
            }
        }

        private static ObscuraXConfig LoadObscuraXConfigStatic()
        {
            var configs = AssetDatabase.FindAssets("t:ObscuraXConfig");
            if (configs.Length > 0)
            {
                var configPath = AssetDatabase.GUIDToAssetPath(configs[0]);
                return AssetDatabase.LoadAssetAtPath<ObscuraXConfig>(configPath);
            }
            return null;
        }

        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            var config = LoadObscuraXConfig();
            if (config == null || !config.EnableObfuscation || !EditorPrefs.GetBool("ObscuraX.Obfuscation", true))
            {
                LogDebug("Obfuscation disabled or config not found");
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            LogDebug("Starting obfuscation process...");

            if (config.UseUnityUIForProtections)
            {
                var enabledProtectionsCount = config.ProtectionSettings?.Count(p => p.Enabled) ?? 0;
                if (enabledProtectionsCount == 0)
                {
                    UnityEngine.Debug.LogError("[ObscuraX] No protections are enabled! Please enable at least one protection in the ObscuraX configuration before building.");
                    throw new BuildFailedException("No protections are enabled. Please enable at least one protection in the ObscuraX configuration.");
                }
                LogDebug($"{enabledProtectionsCount} protection(s) enabled");
            }

            var assemblyPath = GetAssemblyLocation("Assembly-CSharp.dll");
            if (string.IsNullOrEmpty(assemblyPath))
            {
                UnityEngine.Debug.LogError("[ObscuraX] Could not find Assembly-CSharp.dll to obfuscate");
                return;
            }

            LogDebug($"Found assembly at: {assemblyPath}");

            try
            {
                ObfuscateAssembly(assemblyPath, _cancellationTokenSource.Token);
                LogDebug("Obfuscation completed successfully");
            }
            catch (OperationCanceledException)
            {
                LogDebug("Obfuscation was cancelled by user");
                throw new BuildFailedException("Build cancelled by user");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[ObscuraX] Obfuscation failed: {ex}");
                throw new BuildFailedException($"ObscuraX obfuscation failed: {ex.Message}");
            }
            finally
            {
                _currentProcess = null;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private string GetAssemblyLocation(string assemblyName)
        {
            LogDebug($"Looking for assembly: {assemblyName}");

            if (File.Exists(assemblyName) && assemblyName.EndsWith(".dll"))
            {
                LogDebug($"Found assembly at current directory: {assemblyName}");
                return assemblyName;
            }

            if (!assemblyName.EndsWith(".dll"))
            {
                assemblyName += ".dll";
            }

            var possiblePaths = new[]
            {
                Path.Combine(Path.GetDirectoryName(Application.dataPath), "Temp", "StagingArea", "Data", "Managed", assemblyName),
                Path.Combine(Path.GetDirectoryName(Application.dataPath), "Build", "Data", "Managed", assemblyName),
                Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library", "PlayerDataCache", "Data", "Managed", assemblyName),
                Path.Combine(Directory.GetCurrentDirectory(), assemblyName),
                assemblyName
            };

            foreach (var path in possiblePaths)
            {
                LogDebug($"Checking path: {path}");
                if (File.Exists(path))
                {
                    LogDebug($"Found assembly at: {path}");
                    return path;
                }
            }

            UnityEngine.Debug.LogError($"[ObscuraX] Assembly {assemblyName} not found in any expected location");
            return null;
        }

        private void ObfuscateAssembly(string assemblyPath, CancellationToken cancellationToken = default)
        {
            var bitMonoCli = FindObscuraXCli();
            if (string.IsNullOrEmpty(bitMonoCli))
            {
                throw new BuildFailedException("ObscuraX.CLI.exe not found. Please ensure ObscuraX.CLI is built and accessible.");
            }

            var configPath = FindConfigPath();
            if (string.IsNullOrEmpty(configPath))
            {
                throw new BuildFailedException("ObscuraX configuration files not found. Please ensure obfuscation.json, criticals.json, logging.json, and protections.json are available.");
            }

            var config = LoadObscuraXConfig();
            string args;

            LogDebug($"Config loaded UseUnityUI: {config?.UseUnityUIForProtections} ProtectionSettings count: {config?.ProtectionSettings?.Count ?? 0}");
            if (config?.ProtectionSettings != null)
            {
                var enabledCount = config.ProtectionSettings.Count(p => p.Enabled);
                LogDebug($"Enabled protections count: {enabledCount}");
                if (enabledCount > 0)
                {
                    var enabledNames = string.Join(", ", config.ProtectionSettings.Where(p => p.Enabled).Select(p => p.Name));
                    LogDebug($"Enabled protection names: {enabledNames}");
                }
            }

            if (config != null && config.UseUnityUIForProtections && config.ProtectionSettings.Any(p => p.Enabled))
            {
                var enabledProtections = config.ProtectionSettings.Where(p => p.Enabled).Select(p => p.Name);
                var protectionList = string.Join(" ", enabledProtections);
                LogDebug($"Using Unity UI protections: {string.Join(", ", enabledProtections)}");
                args = $"--file \"{assemblyPath}\" --output \"{Path.GetDirectoryName(assemblyPath)}\" " +
                      $"--obfuscation-file \"{Path.Combine(configPath, "obfuscation.json")}\" " +
                      $"--criticals-file \"{Path.Combine(configPath, "criticals.json")}\" " +
                      $"--logging-file \"{Path.Combine(configPath, "logging.json")}\" " +
                      $"--protections {protectionList} --no-watermark";
            }
            else
            {
                LogDebug("Using protections.json file (Unity UI disabled or no protections enabled)");
                args = $"--file \"{assemblyPath}\" --output \"{Path.GetDirectoryName(assemblyPath)}\" " +
                      $"--obfuscation-file \"{Path.Combine(configPath, "obfuscation.json")}\" " +
                      $"--criticals-file \"{Path.Combine(configPath, "criticals.json")}\" " +
                      $"--logging-file \"{Path.Combine(configPath, "logging.json")}\" " +
                      $"--protections-file \"{Path.Combine(configPath, "protections.json")}\" --no-watermark";
            }

            LogDebug($"Running: {bitMonoCli} {args}");

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = bitMonoCli,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(bitMonoCli)
            });

            if (process == null)
            {
                throw new BuildFailedException("Failed to start ObscuraX.CLI process");
            }

            _currentProcess = process;

            var timeout = GetObfuscationTimeout();
            LogDebug($"Starting obfuscation with timeout: {timeout.TotalMinutes} minutes");

            var startTime = DateTime.Now;
            var completed = false;
            var progressTitle = "ObscuraX Obfuscation";
            var progressInfo = "Obfuscating assembly...";

            while (!completed && !process.HasExited)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    LogDebug("Cancellation requested, killing process");
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit(2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Error killing process: {ex.Message}");
                    }
                    EditorUtility.ClearProgressBar();
                    LogDebug("[ObscuraX] Build cancelled by user");
                    throw new BuildFailedException("Build cancelled by user");
                }

                if (DateTime.Now - startTime > timeout)
                {
                    LogDebug($"Timeout reached ({timeout.TotalMinutes} minutes), killing process");
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit(5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Error killing process: {ex.Message}");
                    }
                    EditorUtility.ClearProgressBar();
                    throw new BuildFailedException($"ObscuraX obfuscation timed out after {timeout.TotalSeconds} seconds");
                }

                var elapsed = DateTime.Now - startTime;
                var progress = Math.Min((float)(elapsed.TotalSeconds / timeout.TotalSeconds), 0.99f);

                if (EditorUtility.DisplayCancelableProgressBar(progressTitle, progressInfo, progress))
                {
                    LogDebug("User cancelled obfuscation via progress bar");
                    _cancellationTokenSource?.Cancel();
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit(2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Error killing process: {ex.Message}");
                    }
                    EditorUtility.ClearProgressBar();
                    LogDebug("Build cancelled by user");
                    throw new BuildFailedException("Build cancelled by user");
                }

                Thread.Sleep(1000);
                completed = process.WaitForExit(0);
            }

            EditorUtility.ClearProgressBar();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            var exitCode = process.ExitCode;

            if (!string.IsNullOrEmpty(output))
            {
                LogDebug($"Output: {output}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                LogDebug($"Errors: {error}");
            }

            if (exitCode != 0)
            {
                throw new BuildFailedException($"ObscuraX.CLI failed with exit code {exitCode}. Output: {output}. Error: {error}");
            }
        }

        private async Task ObfuscateAssemblyAsync(string assemblyPath)
        {
            try
            {
                await ObfuscateAssemblyWithCancellation(assemblyPath);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }
        }

        private async Task ObfuscateAssemblyWithCancellation(string assemblyPath)
        {
            var bitMonoCli = FindObscuraXCli();

            if (string.IsNullOrEmpty(bitMonoCli))
            {
                return;
            }

            var configPath = FindConfigPath();

            if (string.IsNullOrEmpty(configPath))
            {
                return;
            }

            var config = LoadObscuraXConfig();
            if (config != null && config.UseUnityUIForProtections)
            {
                config.SaveProtectionsToFile();
            }

            var args = $"--file \"{assemblyPath}\" --output \"{Path.GetDirectoryName(assemblyPath)}\" " +
                      $"--obfuscation-file \"{Path.Combine(configPath, "obfuscation.json")}\" " +
                      $"--criticals-file \"{Path.Combine(configPath, "criticals.json")}\" " +
                      $"--logging-file \"{Path.Combine(configPath, "logging.json")}\" " +
                      $"--protections-file \"{Path.Combine(configPath, "protections.json")}\"";

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = bitMonoCli,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(bitMonoCli)
            });

            if (process != null)
            {
                var timeout = GetObfuscationTimeout();
                var cancellationTokenSource = new CancellationTokenSource();
                var timeoutTokenSource = new CancellationTokenSource(timeout);
                var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutTokenSource.Token);

                try
                {
                    var completed = await WaitForProcessExitAsync(process, combinedTokenSource.Token);

                    if (!completed)
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                process.WaitForExit(5000);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    var exitCode = process.ExitCode;

                    if (exitCode != 0)
                    {
                        LogDebugStatic($"Obfuscation failed with exit code {exitCode} for {Path.GetFileName(assemblyPath)}");
                    }
                }
                catch (OperationCanceledException)
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    throw;
                }
                finally
                {
                    cancellationTokenSource?.Dispose();
                    timeoutTokenSource?.Dispose();
                    combinedTokenSource?.Dispose();
                }
            }
        }

        private async Task<bool> WaitForProcessExitAsync(Process process, CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            var lastLogTime = startTime;

            while (!process.HasExited && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100, cancellationToken);

                var now = DateTime.Now;
                if ((now - lastLogTime).TotalSeconds >= 10)
                {
                    var elapsed = (now - startTime).TotalSeconds;
                    lastLogTime = now;
                }
            }

            return process.HasExited;
        }

        private TimeSpan GetObfuscationTimeout()
        {
            try
            {
                var config = LoadObscuraXConfig();
                if (config != null)
                {
                    return TimeSpan.FromMinutes(config.ObfuscationTimeoutMinutes);
                }
            }
            catch (Exception)
            {
            }
            return TimeSpan.FromMinutes(5);
        }

        private string FindObscuraXCli()
        {
            var paths = new[]
            {
                Path.Combine(Application.dataPath, "..", "ObscuraX.CLI", "ObscuraX.CLI.exe"),
                Path.Combine(Application.dataPath, "..", "..", "ObscuraX.CLI", "ObscuraX.CLI.exe"),
                Path.Combine(Application.dataPath, "..", "..", "src", "ObscuraX.CLI", "bin", "Release", "net462", "ObscuraX.CLI.exe"),
                "ObscuraX.CLI.exe"
            };

            return paths.FirstOrDefault(File.Exists);
        }

        private string FindConfigPath()
        {
            var config = LoadObscuraXConfig();
            if (config != null)
            {
                var detectedPath = config.GetDetectedConfigPath();
                if (!string.IsNullOrEmpty(detectedPath) && !detectedPath.Contains("Not found"))
                {
                    return detectedPath;
                }
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

            return null;
        }

        private ObscuraXConfig LoadObscuraXConfig()
        {
            var configs = AssetDatabase.FindAssets("t:ObscuraXConfig");
            if (configs.Length > 0)
            {
                var configPath = AssetDatabase.GUIDToAssetPath(configs[0]);
                return AssetDatabase.LoadAssetAtPath<ObscuraXConfig>(configPath);
            }
            return null;
        }
    }
}
#endif