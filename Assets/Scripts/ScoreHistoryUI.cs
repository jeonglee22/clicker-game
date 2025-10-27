using System;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreHistoryUI : MonoBehaviour
{
    public Button reloadButton;
    public Button closeButton;

    public ScrollRect scoreRect;
    private RectTransform content;

    public TextMeshProUGUI bestScoreText;

    public GameObject scorePrefab;
    public GameObject gameStartPanel;

    private int bestScore;

    private void OnEnable()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsInitialized)
            return;

        OnReloadHistoryClicked().Forget();
    }

    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        SetButtonsInteractable(true);

        reloadButton.onClick.AddListener(() => OnReloadHistoryClicked().Forget());
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

    private async UniTask OnReloadHistoryClicked()
    {
        SetButtonsInteractable(false);

        DestoryList();

        var scoreList = await ScoreManager.Instance.LoadHistoryAsync();

        foreach (var data in scoreList)
        {
            var score = data.score;
            var scoreBox = Instantiate(scorePrefab, content);
            scoreBox.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0}점 - {1}", score, data.GetDateString());
        }

        bestScoreText.text = string.Format("\t{0}점", ScoreManager.Instance.CachedBestScore);

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
