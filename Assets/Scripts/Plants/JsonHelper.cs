using UnityEngine;

public static class JsonHelper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T> { array = array };
        string jsonWithWrapper = JsonUtility.ToJson(wrapper, prettyPrint);
        // Rimuovi il wrapper {"array": ... } per salvare solo l'array puro
        int startIndex = jsonWithWrapper.IndexOf('[');
        int endIndex = jsonWithWrapper.LastIndexOf(']');
        if (startIndex >= 0 && endIndex >= 0)
        {
            return jsonWithWrapper.Substring(startIndex, endIndex - startIndex + 1);
        }
        return jsonWithWrapper; // fallback
    }
}
