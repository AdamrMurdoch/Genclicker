using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [Header("Backend")]
    [SerializeField] private string baseUrl = "https://genclicker-backend.onrender.com";
    [SerializeField] private bool loadSaveAfterAuth = true;

    private BackendApiClient api;

    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject signupPanel;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField loginUsernameOrEmail;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToSignupButton;
    [SerializeField] private TMP_Text loginErrorText;

    [Header("Signup UI")]
    [SerializeField] private TMP_InputField signupUsername;
    [SerializeField] private TMP_InputField signupEmail;
    [SerializeField] private TMP_InputField signupPassword;
    [SerializeField] private Button signupButton;
    [SerializeField] private Button goToLoginButton;
    [SerializeField] private TMP_Text signupErrorText;

    private void Awake()
    {
        api = new BackendApiClient(baseUrl);

        // wire buttons
        loginButton.onClick.AddListener(OnLoginClicked);
        goToSignupButton.onClick.AddListener(() => ShowSignup(true));

        signupButton.onClick.AddListener(OnSignupClicked);
        goToLoginButton.onClick.AddListener(() => ShowSignup(false));

        // initial state
        ShowSignup(false);

        loginErrorText.text = "";
        signupErrorText.text = "";
    }

    private void ShowSignup(bool showSignup)
    {
        loginPanel.SetActive(!showSignup);
        signupPanel.SetActive(showSignup);

        loginErrorText.text = "";
        signupErrorText.text = "";
    }

    private void SetBusy(bool busy)
    {
        loginButton.interactable = !busy;
        goToSignupButton.interactable = !busy;
        signupButton.interactable = !busy;
        goToLoginButton.interactable = !busy;
    }

    private void OnLoginClicked()
    {
        loginErrorText.text = "";

        string identifier = loginUsernameOrEmail.text.Trim();
        string pass = loginPassword.text;

        if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(pass))
        {
            loginErrorText.text = "Please enter username/email and password.";
            return;
        }

        // Your backend login accepts username OR email (we coded it that way)
        var payload = new LoginRequest
        {
            username = identifier,
            password = pass
        };

        StartCoroutine(LoginRoutine(payload));
    }

    private IEnumerator LoginRoutine(LoginRequest payload)
    {
        SetBusy(true);

        yield return api.Login(
            payload,
            onSuccess: (auth) =>
            {
                // token already saved by BackendApiClient.SetToken()
                loginErrorText.text = "";
                Debug.Log($"Login OK: {auth.user.username}");

                if (loadSaveAfterAuth && SaveSingleton.Instance != null)
                {
                    StartCoroutine(SaveSingleton.Instance.LoadAllData());
                }

                // TODO: Load next scene / hide auth UI
                // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                gameObject.SetActive(false);
            },
            onError: (err) =>
            {
                loginErrorText.text = FriendlyError(err);
                Debug.LogError(err);
            }
        );

        SetBusy(false);
    }

    private void OnSignupClicked()
    {
        signupErrorText.text = "";

        string user = signupUsername.text.Trim();
        string email = signupEmail.text.Trim();
        string pass = signupPassword.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            signupErrorText.text = "Please fill in all fields.";
            return;
        }

        if (!email.Contains("@"))
        {
            signupErrorText.text = "Please enter a valid email.";
            return;
        }

        var payload = new RegisterRequest
        {
            username = user,
            email = email,
            password = pass
        };

        StartCoroutine(SignupRoutine(payload));
    }

    private IEnumerator SignupRoutine(RegisterRequest payload)
    {
        SetBusy(true);

        yield return api.RegisterAccount(
            payload,
            onSuccess: (auth) =>
            {
                signupErrorText.text = "";
                Debug.Log($"Register OK: {auth.user.username}");

                if (loadSaveAfterAuth && SaveSingleton.Instance != null)
                {
                    StartCoroutine(SaveSingleton.Instance.LoadAllData());
                }

                // TODO: Load next scene / hide auth UI
                gameObject.SetActive(false);
            },
            onError: (err) =>
            {
                signupErrorText.text = FriendlyError(err);
                Debug.LogError(err);
            }
        );

        SetBusy(false);
    }

    // Optional: make server errors nicer
    private string FriendlyError(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Unknown error.";

        // common http codes from your api
        if (raw.Contains("400")) return "Incorrect details or account already exists.";
        if (raw.Contains("401")) return "Not authorized. Please log in again.";
        if (raw.Contains("403")) return "Session expired. Please log in again.";
        if (raw.Contains("404")) return "Server endpoint not found.";
        if (raw.Contains("5")) return "Server error. Try again later.";

        return raw;
    }

    // Optional: auto-skip login if token already exists
    public void TryAutoLogin()
    {
        if (!string.IsNullOrEmpty(api.Token))
        {
            StartCoroutine(api.GetUser(
                onSuccess: (user) =>
                {
                    Debug.Log($"Auto-login OK: {user.username}");
                    if (loadSaveAfterAuth && SaveSingleton.Instance != null)
                        StartCoroutine(SaveSingleton.Instance.LoadAllData());

                    gameObject.SetActive(false);
                },
                onError: (err) =>
                {
                    Debug.LogWarning("Auto-login failed: " + err);
                    api.SetToken(null);
                    gameObject.SetActive(true);
                }
            ));
        }
    }
}