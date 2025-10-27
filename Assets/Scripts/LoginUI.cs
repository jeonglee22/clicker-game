using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public Button loginButton;
    public Button authButton;
    public Button anonymousButton;
    public Button profileButton;

    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    private string email;
    private string password;

    public TextMeshProUGUI errorText;
    public TextMeshProUGUI profileText;

    public GameObject loginPanlel;
    public GameObject profileCreatePanlel;
    public GameObject profilePanlel;
    public GameObject nickNameChangePanlel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async UniTaskVoid Start()
    {
        SetButtonsInteractable(false);

        await UniTask.WaitUntil(() => AuthManager.Instance != null && AuthManager.Instance.IsInitialized);
        await UniTask.WaitUntil(() => ProfileManager.Instance != null && ProfileManager.Instance.IsInitialized);

        SetButtonsInteractable(true);

        profileButton.gameObject.SetActive(false);

        loginButton.onClick.AddListener(() => OnEmailLoginClicked().Forget());
        authButton.onClick.AddListener(() => OnCreateAuthClicked().Forget());
        anonymousButton.onClick.AddListener(() => OnGuestLoginClicked().Forget());
        profileButton.onClick.AddListener(() => OnProfileClicked().Forget());

        emailField.onValueChanged.AddListener((str) => email = str);
        passwordField.onValueChanged.AddListener((str) => password = str);

        UpdateUI().Forget();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private async UniTaskVoid OnProfileClicked()
    {
        profilePanlel.SetActive(true);

        // bool isProfileExist = await ProfileManager.Instance.ProfileExistAsync();

        // if (isProfileExist)
        // {
        //     profilePanlel.SetActive(true);
        // }
        // else
        // {
        //     profileCreatePanlel.SetActive(true);
        // }
    }

    public async UniTaskVoid UpdateUI()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsInitialized)
            return;

        bool isLoggedIn = AuthManager.Instance.IsLoggedIn;
        loginPanlel.SetActive(!isLoggedIn);

        if (isLoggedIn)
        {
            bool isProfileExist = await ProfileManager.Instance.ProfileExistAsync();
            if (isProfileExist)
            {
                var result = await ProfileManager.Instance.LoadProfileAsync();
                //string userId  = AuthManager.Instance.UserId;
                profileText.text = result.profile.nickname;
            }
            else
            {
                var newProfile = await ProfileManager.Instance.SaveProfileAsync("Guest");

                if (newProfile.success)
                {
                    profileText.text = "Guest";
                }
                else
                {
                    Debug.Log(newProfile.error);
                }
            }

            // bool isProfileExist = await ProfileManager.Instance.ProfileExistAsync();
            // if (isProfileExist)
            // {
            //     var result = await ProfileManager.Instance.LoadProfileAsync();
            //     //string userId  = AuthManager.Instance.UserId;
            //     profileText.text = result.profile.nickname;
            // }
            // else
            // {
            //     profileText.text = "������";
            // }
            profileButton.gameObject.SetActive(true);

            await ScoreManager.Instance.LoadBestScoreAsync();
        }
        else
        {

        }

        errorText.text = string.Empty;
    }

    private async UniTaskVoid OnEmailLoginClicked()
    {
        if (email == null || password == null)
        {
            Debug.Log("[Auth] ������ �Է����ּ���");
            return;
        }

        SetButtonsInteractable(false);

        var (success, message) = await AuthManager.Instance.SignInwithEmailAsync(email, password);
        if(success)
        {

        }
        else
        {
            ShowError(message);
        }

        SetButtonsInteractable(true);
        UpdateUI().Forget();
    }

    private async UniTaskVoid OnCreateAuthClicked()
    {
        if (email == null || password == null)
        {
            Debug.Log("[Auth] ������ �Է����ּ���");
            return;
        }

        SetButtonsInteractable(false);

        var (success, message) = await AuthManager.Instance.CreateUserwithEmailAsync(email, password);

        if (success)
        {

        }
        else
        {
            ShowError(message);
        }

        SetButtonsInteractable(true);
        UpdateUI().Forget();
    }

    private async UniTaskVoid OnGuestLoginClicked()
    {
        SetButtonsInteractable(false);

        var (success, message) = await AuthManager.Instance.SignInAnonymouslyAsync();

        if (success)
        {
            
        }
        else
        {
            ShowError(message);
        }

        SetButtonsInteractable(true);
        UpdateUI().Forget();
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.color = Color.red;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        loginButton.interactable = interactable;
        authButton.interactable = interactable;
        anonymousButton.interactable = interactable;
    }
}
