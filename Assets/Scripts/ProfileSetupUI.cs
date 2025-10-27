using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class ProfileSetupUI : MonoBehaviour
{
    public Button profileSetButton;
    public Button profileCreateCloseButton;

    public TMP_InputField nickNameCreateField;

    public TextMeshProUGUI profileText;

    private string nickNameCreate;

    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);

        SetButtonsInteractable(true);

        profileSetButton.onClick.AddListener(() => OnProfileSetupClicked().Forget());
        profileCreateCloseButton.onClick.AddListener(() => gameObject.SetActive(false));

        nickNameCreateField.onValueChanged.AddListener(s => nickNameCreate = s);
        nickNameCreateField.text = string.Empty;
    }

    private async UniTaskVoid OnProfileSetupClicked()
    {
        SetButtonsInteractable(false);

        var result = await ProfileManager.Instance.SaveProfileAsync(nickNameCreate);

        if (result.success)
        {
            gameObject.SetActive(false);
            profileText.text = nickNameCreate;
        }
        else
        {
            Debug.Log(result.error);
        }

        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        profileSetButton.interactable = interactable;
        profileCreateCloseButton.interactable = interactable;
    }
}
