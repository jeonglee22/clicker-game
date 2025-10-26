using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NickNameEditUI : MonoBehaviour
{
    public Button nickNameEditButton;
    public Button nickNameEditCloseButton;

    public TMP_InputField nickNameEditField;

    private string nickNameNew;

    public TextMeshProUGUI nickNameBeforeText;

    public GameObject profilePanlel;

    private async UniTaskVoid OnEnable()
    {
        SetButtonsInteractable(false);

        bool isProfileExist = await ProfileManager.Instance.ProfileExistAsync();

        if (isProfileExist)
        {
            (var profile, var error) = await ProfileManager.Instance.LoadProfileAsync();

            nickNameBeforeText.text = profile.nickname;
        }

        SetButtonsInteractable(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        SetButtonsInteractable(true);

        nickNameEditButton.onClick.AddListener(() => OnNickNameEditClicked().Forget());
        nickNameEditCloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            profilePanlel.SetActive(true);
        });

        nickNameEditField.onValueChanged.AddListener(s => nickNameNew = s);
    }

    private async UniTaskVoid OnNickNameEditClicked()
    {
        SetButtonsInteractable(false);

        var result = await ProfileManager.Instance.UpdateNickNameAsync(nickNameNew);
        if (result.success)
        {
            gameObject.SetActive(false);
            profilePanlel.SetActive(true);
        }
        else
        {
            Debug.Log(result.error);
        }

        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        nickNameEditButton.interactable = interactable;
        nickNameEditCloseButton.interactable = interactable;
    }
}
