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

    private void Start()
    {
        seeHistoryButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            scorePanel.SetActive(true); 
        });
        leaderboardButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            leaderboardPanel.SetActive(true);
        });
        titleButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
            gameStartPanel.SetActive(true); 
        });
    }
}
