using UnityEngine;

/// <summary>
/// Controls the draining and refilling animation of the water tank.
/// Gradually reduces the water level by scaling down and adjusting position,
/// then refills after a specified delay.
/// </summary>
public class WaterTankController : MonoBehaviour
{
    [Header("Drain Settings")]
    [Tooltip("Speed at which the water drains (scale reduction per second).")]
    public float drainSpeed = 0.0008f;

    [Tooltip("Minimum allowed height (scale) of the water.")]
    public float minHeight = 0f;

    [Tooltip("Delay in seconds before the water level resets after draining.")]
    public float resetDelay = 1f;

    // Store the initial local scale of the water for resetting later
    private Vector3 initialScale;

    // Store the initial local position of the water for resetting later
    private Vector3 initialPosition;

    // Timer to track elapsed time since draining stopped
    private float resetTimer = 0f;

    // Flag to indicate whether the tank is currently draining
    private bool draining = true;

    /// <summary>
    /// Cache the initial scale and position on start for use during reset.
    /// </summary>
    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    /// <summary>
    /// Called once per frame to handle draining and refilling logic.
    /// </summary>
    void Update()
    {
        if (draining)
        {
            // Calculate new Y scale based on drain speed and delta time
            float currentYScale = transform.localScale.y;
            float newYScale = currentYScale - drainSpeed * Time.deltaTime;
            // Ensure the new scale doesn't go below the minimum height
            newYScale = Mathf.Max(newYScale, minHeight);
            // Calculate how much the scale changed
            float deltaScale = currentYScale - newYScale;
            float deltaPositionY = deltaScale;
            // Update local scale with the new Y value, preserving X and Z
            transform.localScale = new Vector3(initialScale.x, newYScale, initialScale.z);
            // Move the water downward by the amount the scale decreased
            transform.localPosition -= new Vector3(0, deltaPositionY, 0);
            // If water has reached minimum height, stop draining and reset timer
            if (newYScale <= minHeight)
            {
                draining = false;
                resetTimer = 0f;
            }
        }
        else
        {
            // Increment timer until it reaches the reset delay, then reset water level
            if (resetDelay > 0f)
            {
                resetTimer += Time.deltaTime;
                if (resetTimer >= resetDelay)
                {
                    ResetWaterLevel();
                }
            }
            else
            {
                // If no delay is set, reset immediately
                ResetWaterLevel();
            }
        }
    }

    /// <summary>
    /// Resets the water level to the initial scale and position, restarting the draining cycle.
    /// </summary>
    void ResetWaterLevel()
    {
        transform.localScale = initialScale;
        transform.localPosition = initialPosition;
        draining = true;
    }
}
