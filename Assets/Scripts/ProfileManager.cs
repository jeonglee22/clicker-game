using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    private static ProfileManager instance;
    public static ProfileManager Instance => instance;

    private DatabaseReference dataBaseRef;
    private DatabaseReference usersRef;

    private UserProfile cachedProfile;
    public UserProfile CachedProfile => cachedProfile;

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private async UniTaskVoid Start()
    {
        await FireBaseInitializer.Instance.WaitForInitializationAsync();

        dataBaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        usersRef = dataBaseRef.Child("users");

        Debug.Log("[Profile] ProfileManager 초기화 완료");

        isInitialized = true;
    }

    public async UniTask<(bool success, string error)> SaveProfileAsync(string nickname)
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;
        string email = AuthManager.Instance.CurrentUser.Email ?? "익명";

        try
        {
            Debug.Log($"프로필 저장 시도.. {nickname}");

            var profile = new UserProfile(nickname, email);
            var json = profile.ToJson();

            await usersRef.Child(userId).SetRawJsonValueAsync(json).AsUniTask();

            cachedProfile = profile;

            Debug.Log($"프로필 저장 성공... {nickname}");

            return (true,null);

        }
        catch (System.Exception ex)
        {
            Debug.Log($"프로필 저장 실패.. {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> LoadProfileAsync()
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[profile] 프로필 로드 시도...");
            DataSnapshot snapshot = await usersRef.Child(userId).GetValueAsync().AsUniTask();

            if (!snapshot.Exists)
            {
                Debug.Log("[profile] 프로필 없음");
                return (null, "[profile] 프로필 없음");
            }

            string json = snapshot.GetRawJsonValue();
            cachedProfile = UserProfile.FromJson(json);

            Debug.Log($"[profile] 프로필 로드 성공...");

            return (cachedProfile, null);

        }
        catch (System.Exception ex)
        {
            Debug.Log($"[profile] 프로필 저장 실패.. {ex.Message}");
            return (null, ex.Message);
        }
    }

    public async UniTask<(bool success, string error)> UpdateNickNameAsync(string newNickName)
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[profile] 닉네임 변경 시도.. {newNickName}");

            await usersRef.Child(userId).Child("nickname").SetValueAsync(newNickName).AsUniTask();
            cachedProfile.nickname = newNickName;

            await LeaderboardManager.Instance.UpdateLeaderboardAsync(ScoreManager.Instance.CachedBestScore);

            Debug.Log($"[profile] 닉네임 변경 성공... {cachedProfile.nickname}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[profile] 닉네임 변경 실패.. {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<bool> ProfileExistAsync()
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return false;
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await usersRef.Child(userId).GetValueAsync().AsUniTask();
            return snapshot.Exists;
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Profile] 프로필 확인 실패 : {ex.Message}");
            return false;
        }
    }
}
