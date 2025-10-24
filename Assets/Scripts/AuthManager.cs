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
            Debug.Log($"[Auth] 이미 로그인됨 : {UserId}");
        }
        else
        {
            Debug.Log($"[Auth] 로그인이 필요함");
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
            Debug.Log("[Auth] 익명 로그인 시도");

            AuthResult result = await auth.SignInAnonymouslyAsync().AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] 익명 로그인 성공 : {UserId}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] 익명 로그인 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> CreateUserwithEmailAsync(string email, string password)
    {
        try
        {
            Debug.Log("[Auth] 회원 가입 시도");

            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] 회원 가입 성공 : {UserId}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] 회원 가입 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> SignInwithEmailAsync(string email, string password)
    {
        try
        {
            Debug.Log("[Auth] 로그인 시도");

            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] 로그인 성공 : {UserId}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] 로그인 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }

    public void SignOut()
    {
        if (auth != null && currentUser != null)
        {
            Debug.Log("[Auth] 로그아웃");
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
                Debug.Log("[Auth] 로그아웃 됨");
            }

            currentUser = auth.CurrentUser;

            if(siginIn)
            {
                Debug.Log($"[Auth] 로그인 됨 : {UserId}");
            }
        }
    }

    private string ParseFireBaseError(string error)
    {
        return error;
    }
}
