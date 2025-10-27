using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance => instance;

    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private bool isInitialized = false;

    public FirebaseUser CurrentUser => currentUser;
    public bool IsLoggedIn => currentUser != null;
    public string UserId => currentUser?.UserId ?? string.Empty;
    public bool IsInitialized => isInitialized;



    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private async UniTaskVoid Start()
    {
        await FireBaseInitializer.Instance.WaitForInitializationAsync();

        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnAuthStateChanger;

        currentUser = auth.CurrentUser;

        if(currentUser != null)
        {
            Debug.Log($"[Auth] �̹� �α��ε� : {UserId}");
        }
        else
        {
            Debug.Log($"[Auth] �α����� �ʿ���");
        }

        isInitialized = true;
    }

    private void OnDestroy()
    {
        if(auth != null)
        {
            auth.StateChanged -= OnAuthStateChanger;
        }
    }

    public async UniTask<(bool success, string error)> SignInAnonymouslyAsync()
    {
        try
        {
            Debug.Log("[Auth] �͸� �α��� �õ�");

            AuthResult result = await auth.SignInAnonymouslyAsync().AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] �͸� �α��� ���� : {UserId}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] �͸� �α��� ���� : {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> CreateUserwithEmailAsync(string email, string password)
    {
        try
        {
            Debug.Log("[Auth] ȸ�� ���� �õ�");

            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] ȸ�� ���� ���� : {UserId}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] ȸ�� ���� ���� : {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> SignInwithEmailAsync(string email, string password)
    {
        try
        {
            Debug.Log("[Auth] �α��� �õ�");

            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] �α��� ���� : {UserId}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] �α��� ���� : {ex.Message}");
            return (false, ex.Message);
        }
    }

    public void SignOut()
    {
        if (auth != null && currentUser != null)
        {
            Debug.Log("[Auth] �α׾ƿ�");
            auth.SignOut();
            currentUser = null;
            
        }
    }

    private void OnAuthStateChanger(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != currentUser)
        {
            bool siginIn = auth.CurrentUser != currentUser && auth.CurrentUser != null;

            if (!siginIn && currentUser != null)
            { 
                Debug.Log("[Auth] �α׾ƿ� ��");
            }

            currentUser = auth.CurrentUser;

            if(siginIn)
            {
                Debug.Log($"[Auth] �α��� �� : {UserId}");
            }
        }
    }

    private string ParseFireBaseError(string error)
    {
        return error;
    }
}
