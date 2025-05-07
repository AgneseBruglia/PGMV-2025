using UnityEngine;

public class Crossair : MonoBehaviour
{
    public Texture2D crosshairTexture;
    public float crosshairScale = 1;

    private void OnGUI()
    {
        if (crosshairTexture != null)
        {
            float xMin = (Screen.width / 2) - (crosshairTexture.width * crosshairScale / 2);
            float yMin = (Screen.height / 2) - (crosshairTexture.height * crosshairScale / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairTexture.width * crosshairScale, crosshairTexture.height * crosshairScale), crosshairTexture);
        }
    }

}
