using UnityEngine;

/// <summary>
/// Controls the fluid level animation of the blue tank by simulating
/// a draining effect and resetting the fluid level after a delay.
/// </summary>
public class BlueTankController : MonoBehaviour
{
    [Header("Drain Settings")]
    [Tooltip("Speed at which the fluid drains (scale reduction per second).")]
    public float drainSpeed = 0.0005f;

    [Tooltip("Minimum allowed height (scale) of the fluid.")]
    public float minHeight = 0f;

    [Tooltip("Delay in seconds before the fluid level resets after draining.")]
    public float resetDelay = 1f;

    // Original local scale of the tank's fluid (used for resetting)
    private Vector3 initialScale;

    // Original local position of the tank's fluid (used for resetting)
    private Vector3 initialPosition;

    // Timer to track elapsed time since draining finished
    private float resetTimer = 0f;

    // Flag to indicate whether the tank is currently draining
    private bool draining = true;

    /// <summary>
    /// Cache the initial scale and position on start for reset purposes.
    /// </summary>
    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    /// <summary>
    /// Update is called once per frame. Handles draining and resetting the fluid level.
    /// </summary>
    void Update()
    {
        if (draining)
        {
            // Calculate new Y scale based on drain speed and elapsed time
            float currentYScale = transform.localScale.y;
            float newYScale = currentYScale - drainSpeed * Time.deltaTime;
            // Clamp the scale so it doesn't go below the minimum height
            newYScale = Mathf.Max(newYScale, minHeight);
            // Calculate the change in scale and adjust position accordingly to keep visual consistency
            float deltaScale = currentYScale - newYScale;
            float deltaPositionY = deltaScale;
            // Update local scale with the new Y value while preserving X and Z
            transform.localScale = new Vector3(initialScale.x, newYScale, initialScale.z);
            // Lower the local position by the change in scale to simulate draining downward
            transform.localPosition -= new Vector3(0, deltaPositionY, 0);
            // If the fluid level has reached or passed the minimum height, stop draining and start reset timer
            if (newYScale <= minHeight)
            {
                draining = false;
                resetTimer = 0f;
            }
        }
        else
        {
            // Handle delay before resetting fluid level
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
    /// Resets the fluid level to its original scale and position,
    /// and restarts the draining process.
    /// </summary>
    void ResetFluidLevel()
    {
        transform.localScale = initialScale;
        transform.localPosition = initialPosition;
        draining = true;
    }
}
