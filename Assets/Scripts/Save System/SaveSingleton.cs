using UnityEngine;
using System.Collections;

public class SaveSingleton : MonoBehaviour
{
    public static SaveSingleton Instance { get; private set; }

    // Backend API root URL (for example: "http://localhost:3000" when running locally)
    [SerializeField] private string baseUrl = "https://exampleurl.com";

    public SaveData Save { get; private set; } = new SaveData();

    private BackendApiClient BackendApi;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BackendApi = new BackendApiClient(baseUrl);
    }

    // Called via StartCoroutine(SaveSingleton.Instance.LoadData());
    public IEnumerator LoadData()
    {
        yield return BackendApi.GetSave(onSuccess: (payload) =>
            {
                if (payload == null)
                {
                    Save = new SaveData();
                    return;
                }

                Save = payload.save ?? new SaveData();
            },
            onError: (err) =>
            {
                Debug.LogError($"LoadData failed: {err}");
                Save = new SaveData();
            }
        );
    }

    // Called via StartCoroutine(SaveSingleton.Instance.SaveData());
    public IEnumerator SaveData()
    {
        Save.updatedAt = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var payload = new SavePayload
        {
            version = 1,
            save = Save
        };

        yield return BackendApi.PutSave(
            payload,
            onSuccess: () => { },
            onError: (err) => Debug.LogError($"SaveData failed: {err}")
        );
    }
}