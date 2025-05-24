using UnityEngine;

public class WindEffect : MonoBehaviour
{
    public bool windEnabled = true;
    public float amplitude = 15f;
    public float frequency = 1.5f;
    private Vector3 initialRotation;

    void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        // Checks whether the wind effect is on or not
        if (!windEnabled) return;

        float angle = Mathf.Sin(Time.time * frequency + transform.GetInstanceID()) * amplitude;
        transform.localRotation = Quaternion.Euler(initialRotation + new Vector3(0, angle, 0));
    }
}
