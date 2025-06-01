using UnityEngine;

public class InstructionsController : MonoBehaviour
{
    public GameObject instructionsCanvas;
    public PlayerView playerView;
    public PlayerController playerController;
    public Crossair crosshairScript;

    void Start()
    {
        if (playerView == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                playerView = mainCam.GetComponent<PlayerView>();
            crosshairScript = mainCam.GetComponent<Crossair>();
        }
        CloseUI();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenUI();
        }
    }

    public void OpenUI()
    {
        instructionsCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerView != null)
            playerView.enabled = false;

        if (playerController != null)
            playerController.enabled = false;

        if (crosshairScript != null)
            crosshairScript.enabled = false;
    }

    public void CloseUI()
    {
        instructionsCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        if (playerView != null)
            playerView.enabled = true;

        if (playerController != null)
            playerController.enabled = true;

        if (crosshairScript != null)
            crosshairScript.enabled = true;
    }
}