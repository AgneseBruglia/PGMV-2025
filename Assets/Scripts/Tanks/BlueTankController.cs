using UnityEngine;

public class BlueTankController : MonoBehaviour
{
    // Draining speed
    public float drainSpeed = 0.0005f;       
    // Minimum height
    public float minHeight = 0f;       
    // Refill delay
    public float resetDelay = 1f;          

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private float resetTimer = 0f;
    private bool draining = true;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (draining)
        {
            float currentYScale = transform.localScale.y;
            float newYScale = currentYScale - drainSpeed * Time.deltaTime;

            newYScale = Mathf.Max(newYScale, minHeight);

            // Position update
            float deltaScale = currentYScale - newYScale;
            float deltaPositionY = deltaScale;

            transform.localScale = new Vector3(initialScale.x, newYScale, initialScale.z);
            transform.localPosition -= new Vector3(0, deltaPositionY, 0);

            // Check for minimum height
            if (deltaPositionY <= minHeight)
            {
                draining = false;
                resetTimer = 0f;
            }
        }
        else
        {
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
                ResetWaterLevel();
            }
        }
    }

    void ResetWaterLevel()
    {
        transform.localScale = initialScale;
        transform.localPosition = initialPosition;
        draining = true;
    }
}
