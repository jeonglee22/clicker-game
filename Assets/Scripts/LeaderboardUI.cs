using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public Button reloadButton;
    public Button closeButton;

    public ScrollRect scoreRect;
    private RectTransform content;

    public TextMeshProUGUI bestPlayerCountText;

    public Toggle liveUpdateToggle;

    public GameObject leaderBoardScorePrefab;
    public GameObject gameStartPanel;

    private bool liveUpdate;
    private int limitCount = 10;

    private void OnEnable()
    {
        OnReloadLeaderboardClicked().Forget();
    }

    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        SetButtonsInteractable(true);

        reloadButton.onClick.AddListener(() => OnReloadLeaderboardClicked().Forget());
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        content = scoreRect.content;
    }

    private void SetButtonsInteractable(bool v)
    {
        reloadButton.interactable = v;
        closeButton.interactable = v;
    }

    private async UniTaskVoid OnReloadLeaderboardClicked()
    {
        SetButtonsInteractable(false);

        DestoryList();

        await LeaderboardManager.Instance.LoadLeaderboardAsync();

        var leaderboardList = LeaderboardManager.Instance.CachedLeaderboard;

        for (int i = 0; i < leaderboardList.Count; i++)
        {
            var score = leaderboardList[i].score;
            var scoreBox = Instantiate(leaderBoardScorePrefab, content);
            scoreBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
            scoreBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = leaderboardList[i].nickname;
            scoreBox.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = leaderboardList[i].score.ToString();

            scoreBox.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0}점 - {1}", score, leaderboardList[i].GetDateString());
        }

        bestPlayerCountText.text = string.Format("\t{0}점", LeaderboardManager.Instance.CachedLeaderboard.Count);


        SetButtonsInteractable(true);
    }

    private void DestoryList()
    {
        if (content == null) return;

        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i).gameObject;
            Destroy(child);
        }
    }
}
