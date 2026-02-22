using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField usernameOrEmailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text errorText;

    [Header("Navigation")]
    [SerializeField] private string sceneAfterLogin = "GameScene";
    [SerializeField] private string signupSceneName = "SignupScene";
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    private BackendApiClient api;

    private void Awake()
    {
        api = new BackendApiClient(BackendConfig.BaseUrl);
        errorText.text = "";
    }

    public void GoToSignup()
    {
        sceneTransitionManager.OnSceneChanged("Create Account");
    }

    public void OnLoginClicked()
    {
        errorText.text = "";

        var identifier = usernameOrEmailInput.text.Trim();
        var pass = passwordInput.text;

        if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(pass))
        {
            errorText.text = "Enter username/email and password.";
            return;
        }

        // backend supports username OR email in this field
        var payload = new LoginRequest { username = identifier, password = pass };
        StartCoroutine(LoginRoutine(payload));
    }

    private IEnumerator LoginRoutine(LoginRequest payload)
    {
        SetBusy(true);

        yield return api.Login(
            payload,
            onSuccess: (auth) =>
            {
                // token saved by api.SetToken inside BackendApiClient
                errorText.text = "";
                StartCoroutine(SaveSingleton.Instance.LoadAllData());
                sceneTransitionManager.OnSceneChanged("Home");
            },
            onError: (err) =>
            {
                errorText.text = FriendlyError(err);
            }
        );

        SetBusy(false);
    }

    private void SetBusy(bool busy)
    {
        loginButton.interactable = !busy;
    }

    private string FriendlyError(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Unknown error.";
        if (raw.Contains("400")) return "Incorrect username/email or password.";
        if (raw.Contains("403")) return "Session expired. Please log in again.";
        if (raw.Contains("5")) return "Server error. Try again later.";
        return raw;
    }
}