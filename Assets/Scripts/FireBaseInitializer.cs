using UnityEngine;
using Firebase;
using Cysharp.Threading.Tasks;

public class FireBaseInitializer : MonoBehaviour
{
    private static FireBaseInitializer instance;
    public static FireBaseInitializer Instance => instance;

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private FirebaseApp firebaseApp;
    public FirebaseApp FirebaseApp => firebaseApp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeFirebaseAsync().Forget();
    }

    private void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }

    private async UniTaskVoid InitializeFirebaseAsync()
    {
        Debug.Log("[Firebase] 초기화 시작");

        try
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if(status == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                isInitialized = true;

                Debug.Log($"[firebase] 초기화 성공! {firebaseApp.Name}");
            }
            else
            {
                Debug.Log($"[firebase] 초기화 오류 : {status}");
                isInitialized = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[firebase] 초기화 오류 : {ex.Message}");
            isInitialized = false;
        }
    }

    public async UniTask WaitForInitializationAsync()
    {
        await UniTask.WaitUntil(() => isInitialized);
    }
}
