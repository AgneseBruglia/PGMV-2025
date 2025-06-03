using UnityEngine;

/// <summary>
/// A data structure representing the configurable properties of a procedural plant.
/// This struct can be used to store or transfer plant generation parameters,
/// making it useful for saving/loading presets or communicating with UI systems.
/// </summary>
[System.Serializable] // Allows this struct to be visible and editable in the Unity Inspector (e.g., in custom editors or scriptable objects)
public struct Plant
{
    /// <summary>
    /// The number of L-System iterations to perform.
    /// Higher values produce more complex and detailed plants.
    /// </summary>
    public int iterations;

    /// <summary>
    /// The name of the JSON file that contains the L-System rule set.
    /// Used to determine the generation logic for the plant.
    /// </summary>
    public string ruleFileName;

    /// <summary>
    /// A general scaling factor applied to all generated plant parts (branches, leaves, flowers).
    /// </summary>
    public float scale;

    /// <summary>
    /// The base length of each branch segment.
    /// Affects how long each 'F' command in the L-System results in visible geometry.
    /// </summary>
    public float branchLength;

    /// <summary>
    /// The angle in degrees used when rotating the "turtle" interpreter for directional commands.
    /// Affects the curvature and spread of the plant structure.
    /// </summary>
    public float delta;

    /// <summary>
    /// Probability (0 to 1) of spawning a flower at the end of a branch.
    /// Controls how frequently flowers are generated.
    /// </summary>
    public float flowerSpawnProbability;
}
