using UnityEngine;

/// <summary>
/// Simulates a wind effect by oscillating an object's local Y-axis rotation over time.
/// Can be attached to branches, leaves, or flowers to add dynamic motion and realism.
/// </summary>
public class WindEffect : MonoBehaviour
{
    /// <summary>
    /// Enables or disables the wind animation at runtime.
    /// </summary>
    public bool windEnabled = true;

    /// <summary>
    /// The maximum rotational displacement (in degrees) caused by the wind.
    /// Higher values result in wider sway arcs.
    /// </summary>
    public float amplitude = 15f;

    /// <summary>
    /// Controls the speed of the animation.
    /// Higher values result in faster oscillation.
    /// </summary>
    public float frequency = 1.5f;

    // Stores the object's original local rotation (in Euler angles) to use as a base for animation
    private Vector3 initialRotation;

    /// <summary>
    /// Caches the initial rotation when the component starts.
    /// This ensures that wind sway is applied relative to the original orientation.
    /// </summary>
    void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    /// <summary>
    /// Applies a sine wave to the Y-axis rotation to simulate wind swaying motion.
    /// Each instance is offset using its unique instance ID to prevent synchronized movement.
    /// </summary>
    void Update()
    {
        // Skip wind logic if the effect is disabled
        if (!windEnabled) return;
        // Create a time-based angle using a sine wave with unique phase per instance
        float angle = Mathf.Sin(Time.time * frequency + transform.GetInstanceID()) * amplitude;
        // Apply the rotation around the Y-axis while preserving the original X and Z angles
        transform.localRotation = Quaternion.Euler(initialRotation + new Vector3(0, angle, 0));
    }
}
