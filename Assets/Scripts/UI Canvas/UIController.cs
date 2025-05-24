using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject uiCanvas;
    public PlayerView playerView;
    public PlayerController playerController;

    private bool isUIOpen = false;

    void Start()
    {
        if (playerView == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                playerView = mainCam.GetComponent<PlayerView>();
        }
        CloseUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isUIOpen)
                CloseUI();
            else
                OpenUI();
        }
    }

    public void OpenUI()
    {
        uiCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isUIOpen = true;

        // Disables the player movement and interactions
        if (playerView != null)
            playerView.enabled = false;

        if (playerController != null)
            playerController.enabled = false;
    }

    public void CloseUI()
    {
        uiCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isUIOpen = false;

        // Enables the player movement and interactions
        if (playerView != null)
            playerView.enabled = true;

        if (playerController != null)
            playerController.enabled = true;
    }
}
