using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject highScoresPanel;

    public void Play() => SceneManager.LoadScene(1);

    public void ViewHighScores()
    {
        mainMenuPanel.SetActive(false);
        highScoresPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        mainMenuPanel.SetActive(true);
        highScoresPanel.SetActive(false);
    }

    public void Exit() => Application.Quit();
}
