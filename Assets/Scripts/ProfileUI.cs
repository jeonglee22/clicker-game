using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    public Button profileButton;
    public Button nickNameEditStartButton;
    public Button logoutButton;
    public Button profileCloseButton;

    public TextMeshProUGUI nickNameCurrentText;
    public TextMeshProUGUI userIdText;
    public TextMeshProUGUI profileText;

    public GameObject loginPanlel;
    public GameObject nickNameChangePanlel;
    public GameObject gameOverPanel;

    private async UniTaskVoid OnEnable()
    {
        SetButtonsInteractable(false);

        await ProfileSet();

        SetButtonsInteractable(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        SetButtonsInteractable(true);

        nickNameEditStartButton.onClick.AddListener(() => OnNickNameEditStartClicked().Forget());
        logoutButton.onClick.AddListener(() => OnLogoutClicked());

        profileCloseButton.onClick.AddListener(() => gameObject.SetActive(false));

        await ProfileSet();
    }

    private async UniTask ProfileSet()
    {
        bool isProfileExist = await ProfileManager.Instance.ProfileExistAsync();

        if (isProfileExist)
        {
            (var profile, var error) = await ProfileManager.Instance.LoadProfileAsync();

            nickNameCurrentText.text = profile.nickname;
            profileText.text = profile.nickname;
            userIdText.text = AuthManager.Instance.UserId;
        }
        else
        {
            
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        nickNameEditStartButton.interactable = interactable;
        logoutButton.interactable = interactable;
        profileCloseButton.interactable = interactable;
    }

    private async UniTaskVoid OnNickNameEditStartClicked()
    {
        gameObject.SetActive(false);
        nickNameChangePanlel.SetActive(true);
    }

    private void OnLogoutClicked()
    {
        SetButtonsInteractable(false);

        AuthManager.Instance.SignOut();
        loginPanlel.SetActive(true);
        gameObject.SetActive(false);
        profileButton.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);

        SetButtonsInteractable(true);
    }
}
