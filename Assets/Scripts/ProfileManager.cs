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

        Debug.Log("[Profile] ProfileManager �ʱ�ȭ �Ϸ�");

        isInitialized = true;
    }

    public async UniTask<(bool success, string error)> SaveProfileAsync(string nickname)
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] �α��� X");
        }

        string userId = AuthManager.Instance.UserId;
        string email = AuthManager.Instance.CurrentUser.Email ?? "�͸�";

        try
        {
            Debug.Log($"������ ���� �õ�.. {nickname}");

            var profile = new UserProfile(nickname, email);
            var json = profile.ToJson();

            await usersRef.Child(userId).SetRawJsonValueAsync(json).AsUniTask();

            cachedProfile = profile;

            Debug.Log($"������ ���� ����... {nickname}");

            return (true,null);

        }
        catch (System.Exception ex)
        {
            Debug.Log($"������ ���� ����.. {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> LoadProfileAsync()
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] �α��� X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[profile] ������ �ε� �õ�...");
            DataSnapshot snapshot = await usersRef.Child(userId).GetValueAsync().AsUniTask();

            if (!snapshot.Exists)
            {
                Debug.Log("[profile] ������ ����");
                return (null, "[profile] ������ ����");
            }

            string json = snapshot.GetRawJsonValue();
            cachedProfile = UserProfile.FromJson(json);

            Debug.Log($"[profile] ������ �ε� ����...");

            return (cachedProfile, null);

        }
        catch (System.Exception ex)
        {
            Debug.Log($"[profile] ������ ���� ����.. {ex.Message}");
            return (null, ex.Message);
        }
    }

    public async UniTask<(bool success, string error)> UpdateNickNameAsync(string newNickName)
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] �α��� X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[profile] �г��� ���� �õ�.. {newNickName}");

            await usersRef.Child(userId).Child("nickname").SetValueAsync(newNickName).AsUniTask();
            cachedProfile.nickname = newNickName;

            await LeaderboardManager.Instance.UpdateLeaderboardAsync(ScoreManager.Instance.CachedBestScore);

            Debug.Log($"[profile] �г��� ���� ����... {cachedProfile.nickname}");

            return (true, null);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[profile] �г��� ���� ����.. {ex.Message}");
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
            Debug.Log($"[Profile] ������ Ȯ�� ���� : {ex.Message}");
            return false;
        }
    }
}
