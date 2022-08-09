using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUiManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text finalScoreLabel;

    private void Start()
    {
        PlayerStats.Instance.PlayerDied += PlayerStats_PlayerDied;
    }

    private void PlayerStats_PlayerDied(object sender, System.EventArgs e)
    {
        finalScoreLabel.text = $"Your score is: {PlayerStats.Instance.Score:F3}";
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.Restart();
    }

    public void GoToMainMenu() => SceneManager.LoadScene(0);
}
