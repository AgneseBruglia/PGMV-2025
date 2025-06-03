using UnityEngine;

/// <summary>
/// Controls the fluid draining and refilling animation for the pink tank.
/// Simulates gradual draining by scaling down and moves position accordingly,
/// then refills after a delay.
/// </summary>
public class PinkTankController : MonoBehaviour
{
    [Header("Drain Settings")]
    [Tooltip("Speed at which the fluid drains (scale reduction per second).")]
    public float drainSpeed = 0.0008f;

    [Tooltip("Minimum allowed height (scale) of the fluid.")]
    public float minHeight = 0f;

    [Tooltip("Delay in seconds before the fluid level resets after draining.")]
    public float resetDelay = 1f;

    // Store the initial local scale of the tank's fluid for resetting
    private Vector3 initialScale;

    // Store the initial local position of the tank's fluid for resetting
    private Vector3 initialPosition;

    // Timer for counting time elapsed since draining finished
    private float resetTimer = 0f;

    // Flag to track whether the tank is currently draining
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
    /// Called every frame. Handles the draining and refilling logic.
    /// </summary>
    void Update()
    {
        if (draining)
        {
            // Calculate the new Y scale based on drain speed and time passed
            float currentYScale = transform.localScale.y;
            float newYScale = currentYScale - drainSpeed * Time.deltaTime;
            // Prevent the scale from going below the minimum height
            newYScale = Mathf.Max(newYScale, minHeight);
            // Calculate how much the scale has changed
            float deltaScale = currentYScale - newYScale;
            float deltaPositionY = deltaScale;
            // Update local scale with the new Y scale, keep X and Z unchanged
            transform.localScale = new Vector3(initialScale.x, newYScale, initialScale.z);
            // Adjust position downwards by the amount the scale shrunk to simulate draining
            transform.localPosition -= new Vector3(0, deltaPositionY, 0);
            // If the fluid level reached or passed the minimum height, stop draining and start the reset timer
            if (newYScale <= minHeight)
            {
                draining = false;
                resetTimer = 0f;
            }
        }
        else
        {
            // Count up the timer until it reaches the reset delay
            if (resetDelay > 0f)
            {
                resetTimer += Time.deltaTime;
                if (resetTimer >= resetDelay)
                {
                    ResetFluidLevel();
                }
            }
            else
            {
                // If no delay set, reset immediately
                ResetFluidLevel();
            }
        }
    }

    /// <summary>
    /// Resets the fluid level to the original scale and position,
    /// then restarts the draining cycle.
    /// </summary>
    void ResetFluidLevel()
    {
        transform.localScale = initialScale;
        transform.localPosition = initialPosition;
        draining = true;
    }
}
