using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log(finalScoreLabel is not null);
        finalScoreLabel.text = $"Your score is: { PlayerStats.Instance.Score }";
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.Restart();
    }
    
    public void GoToMainMenu()
    {
        Debug.Log("Moving to main menu!");
    }
}
