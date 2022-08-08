using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUiManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPopup;
    [SerializeField] private Button pauseButton;

    private bool _paused = false;

    private void Awake()
    {
        GameManager.Instance.LevelLoaded += (_, _) => pauseButton.interactable = true;
        PlayerStats.Instance.PlayerDied += (_, _) => pauseButton.interactable = false;
    }

    public void TogglePause()
    {
        _paused ^= true;
        pauseMenuPopup.SetActive(_paused);
        pauseButton.interactable = !_paused;

        if (_paused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void RestartGame()
    {
        TogglePause();
        GameManager.Instance.Restart();
    }

    public void GoToMainMenu() => Debug.Log("Moving to main menu!");
}
