using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button seeHistoryButton;
    public Button leaderboardButton;
    public Button titleButton;

    public GameObject scorePanel;
    public GameObject leaderboardPanel;
    public GameObject gameStartPanel;
    public GameObject gameOverPanel;

    private void Start()
    {
        seeHistoryButton.onClick.AddListener(() =>
        {
            scorePanel.SetActive(true); 
        });
        leaderboardButton.onClick.AddListener(() =>
        {
            leaderboardPanel.SetActive(true);
        });
        titleButton.onClick.AddListener(() => {
            gameOverPanel.SetActive(false);
            gameStartPanel.SetActive(true); 
        });
    }
}
