using UnityEngine;
using System.IO;
using System.Collections;

/// <summary>
/// Static utility class for managing rule files used by the plant generation system.
/// Handles file existence checks, path construction, and copying files from StreamingAssets.
/// </summary>
public static class RuleFileManager
{
    // Folder name where rule files are stored in the persistent data path
    private static readonly string rulesFolderName = "Rules";
    /// <summary>
    /// Gets the full path to the rules folder inside the persistent data directory.
    /// If the folder doesn't exist, it is created.
    /// </summary>
    /// <returns>Absolute path to the rules directory</returns>
    public static string GetRulesPath()
    {
        string path = Path.Combine(Application.persistentDataPath, rulesFolderName);
        // Ensure the directory exists
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    /// <summary>
    /// Checks if a specific rule file exists in the persistent rules folder.
    /// </summary>
    /// <param name="fileName">Name of the rule file to check</param>
    /// <returns>True if the file exists, false otherwise</returns>
    public static bool RuleFileExists(string fileName)
    {
        string filePath = Path.Combine(GetRulesPath(), fileName);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Gets the full path to a specific rule file in the persistent data directory.
    /// </summary>
    /// <param name="fileName">Name of the rule file</param>
    /// <returns>Absolute file path</returns>
    public static string GetRuleFilePath(string fileName)
    {
        return Path.Combine(GetRulesPath(), fileName);
    }

    /// <summary>
    /// Coroutine that copies a list of rule files from StreamingAssets to the persistent data path.
    /// This handles platform differences, especially for Android.
    /// </summary>
    /// <param name="fileNames">Array of file names to copy</param>
    /// <returns>IEnumerator for coroutine</returns>
    public static IEnumerator CopyRulesFromStreamingAssets(string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            string destinationPath = GetRuleFilePath(fileName);
            // Skip if the file already exists at the destination
            if (!File.Exists(destinationPath))
            {
                string sourcePath = Path.Combine(Application.streamingAssetsPath, "Rules", fileName);
#if UNITY_ANDROID && !UNITY_EDITOR
                // On Android, StreamingAssets are accessed via UnityWebRequest
                UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(sourcePath);
                yield return request.SendWebRequest();
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    // Write downloaded data to file
                    File.WriteAllBytes(destinationPath, request.downloadHandler.data);
                }
                else
                {
                    Debug.LogError($"Error copying file from {sourcePath}: {request.error}");
                }
#else
                // On other platforms, direct file access is allowed
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destinationPath);
                }
                else
                {
                    Debug.LogWarning($"File not found in StreamingAssets: {sourcePath}");
                }
#endif
            }
        }
        // Yield once at the end of the coroutine
        yield return null;
    }
}
