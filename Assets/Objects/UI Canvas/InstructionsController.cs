using UnityEngine;

/// <summary>
/// Manages the display of the instructions UI canvas and controls player interaction states
/// when the instructions are open or closed.
/// </summary>
public class InstructionsController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Canvas GameObject that contains the instructions UI.")]
    public GameObject instructionsCanvas;

    [Header("Player Components")]
    [Tooltip("Reference to the PlayerView script (usually attached to the main camera).")]
    public PlayerView playerView;

    [Tooltip("Reference to the PlayerController script.")]
    public PlayerController playerController;

    [Tooltip("Reference to the Crosshair script.")]
    public Crossair crosshairScript;

    /// <summary>
    /// Initialization logic to find missing references and start with the instructions UI closed.
    /// </summary>
    void Start()
    {
        // If playerView is not assigned, try to get it from the main camera
        if (playerView == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                playerView = mainCam.GetComponent<PlayerView>();
                crosshairScript = mainCam.GetComponent<Crossair>();
            }
        }
        // Ensure the instructions UI is closed at start
        CloseUI();
    }

    /// <summary>
    /// Listens for the 'C' key press to open the instructions UI.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenUI();
        }
    }

    /// <summary>
    /// Opens the instructions UI canvas and disables player interaction scripts.
    /// Also unlocks and shows the cursor for UI interaction.
    /// </summary>
    public void OpenUI()
    {
        instructionsCanvas.SetActive(true);
        // Unlock the cursor and make it visible for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Disable player control scripts while instructions are visible
        if (playerView != null)
            playerView.enabled = false;
        if (playerController != null)
            playerController.enabled = false;
        if (crosshairScript != null)
            crosshairScript.enabled = false;
    }

    /// <summary>
    /// Closes the instructions UI canvas and re-enables player interaction scripts.
    /// Locks the cursor back for gameplay.
    /// </summary>
    public void CloseUI()
    {
        instructionsCanvas.SetActive(false);
        // Lock the cursor and hide it for normal gameplay control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        // Re-enable player control scripts after UI is closed
        if (playerView != null)
            playerView.enabled = true;
        if (playerController != null)
            playerController.enabled = true;
        if (crosshairScript != null)
            crosshairScript.enabled = true;
    }
}
