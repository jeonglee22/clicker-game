using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    public Button seeHistoryButton;
    public Button leaderboardButton;

    public GameObject scorePanel;
    public GameObject leaderboardPanel;
    public GameObject gameStartPanel;

    private void Start()
    {
        seeHistoryButton.onClick.AddListener(() =>
        {
            scorePanel.SetActive(true); 
        });
        leaderboardButton.onClick.AddListener(() => {
            leaderboardPanel.SetActive(true); 
        });
    }
}
