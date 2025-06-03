using UnityEngine;

/// <summary>
/// A utility class for serializing and deserializing JSON arrays using Unity's JsonUtility,
/// which does not natively support top-level JSON arrays.
/// This class wraps and unwraps arrays in a serializable object for compatibility.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Wrapper class used to encapsulate an array inside an object.
    /// This is required because Unity's JsonUtility does not support direct array serialization/deserialization.
    /// </summary>
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    /// <summary>
    /// Deserializes a JSON string into an array of type T.
    /// Internally wraps the input JSON in an object with a key "array" to make it compatible with JsonUtility.
    /// </summary>
    /// <typeparam name="T">The type of elements in the resulting array.</typeparam>
    /// <param name="json">The raw JSON string representing an array.</param>
    /// <returns>An array of deserialized objects.</returns>
    public static T[] FromJson<T>(string json)
    {
        // Inject wrapper key so JsonUtility can parse it
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    /// <summary>
    /// Serializes an array of type T into a JSON string.
    /// Optionally pretty-prints the result. Removes the internal wrapper so only the raw array is returned.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to serialize.</param>
    /// <param name="prettyPrint">Whether to format the output for readability.</param>
    /// <returns>A JSON string representing the array, without any wrapping object.</returns>
    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        // Wrap the array in a container for compatibility with JsonUtility
        Wrapper<T> wrapper = new Wrapper<T> { array = array };
        string jsonWithWrapper = JsonUtility.ToJson(wrapper, prettyPrint);
        // Strip the wrapper and return only the array portion of the JSON
        int startIndex = jsonWithWrapper.IndexOf('[');
        int endIndex = jsonWithWrapper.LastIndexOf(']');
        if (startIndex >= 0 && endIndex >= 0)
        {
            return jsonWithWrapper.Substring(startIndex, endIndex - startIndex + 1);
        }
        // Fallback: return the wrapped JSON in case of an unexpected format
        return jsonWithWrapper;
    }
}
