using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    public Button seeHistoryButton;
    public Button leaderboardButton;

    public GameObject scorePanel;
    public GameObject leaderboardPanel;

    private void Start()
    {
        seeHistoryButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            scorePanel.SetActive(true); 
        });
        leaderboardButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
            leaderboardPanel.SetActive(true); 
        });
    }
}
