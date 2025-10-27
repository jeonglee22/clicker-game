using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
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
    private List<LeaderboardData> leaderboardList;

    private async UniTaskVoid OnEnable()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsInitialized)
            return;

        await OnReloadLeaderboardClicked();
        AddContent();
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
        AddContent();
        // await OnReloadLeaderboardClicked();
    }

    private void AddContent()
    {
        if (leaderboardList == null) return;

        for (int i = 0; i < leaderboardList.Count; i++)
        {
            var score = leaderboardList[i].score;
            var scoreBox = Instantiate(leaderBoardScorePrefab, content);
            var texts = scoreBox.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (i + 1).ToString();
            texts[1].text = leaderboardList[i].nickname;
            texts[2].text = score.ToString();
        }

        bestPlayerCountText.text = string.Format("\t{0}명", LeaderboardManager.Instance.CachedLeaderboard.Count);

    }

    private void SetButtonsInteractable(bool v)
    {
        reloadButton.interactable = v;
        closeButton.interactable = v;
    }

    private async UniTask OnReloadLeaderboardClicked()
    {
        SetButtonsInteractable(false);

        DestoryList();

        await LeaderboardManager.Instance.LoadLeaderboardAsync();

        leaderboardList = LeaderboardManager.Instance.CachedLeaderboard;

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
