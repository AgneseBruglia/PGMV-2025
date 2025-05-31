using UnityEngine;
using System.IO;
using System.Collections;

public static class RuleFileManager
{
    private static readonly string rulesFolderName = "Rules";

    public static string GetRulesPath()
    {
        string path = Path.Combine(Application.persistentDataPath, rulesFolderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public static bool RuleFileExists(string fileName)
    {
        string filePath = Path.Combine(GetRulesPath(), fileName);
        return File.Exists(filePath);
    }

    public static string GetRuleFilePath(string fileName)
    {
        return Path.Combine(GetRulesPath(), fileName);
    }

    public static IEnumerator CopyRulesFromStreamingAssets(string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            string destinationPath = GetRuleFilePath(fileName);

            if (!File.Exists(destinationPath))
            {
                string sourcePath = Path.Combine(Application.streamingAssetsPath, "Rules", fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
                UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(sourcePath);
                yield return request.SendWebRequest();

                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(destinationPath, request.downloadHandler.data);
                }
                else
                {
                    Debug.LogError($"Errore nel copiare il file da {sourcePath}: {request.error}");
                }
#else
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destinationPath);
                }
                else
                {
                    Debug.LogWarning($"File non trovato in StreamingAssets: {sourcePath}");
                }
#endif
            }
        }

        yield return null;
    }
}
