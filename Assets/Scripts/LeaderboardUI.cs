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

    public TextMeshProUGUI bestScoreText;

    public GameObject scorePrefab;
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
        await FireBaseInitializer.Instance.WaitForInitializationAsync();

        SetButtonsInteractable(true);

        reloadButton.onClick.AddListener(() => OnReloadLeaderboardClicked().Forget());
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            gameStartPanel.SetActive(true);
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
