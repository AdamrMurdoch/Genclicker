using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignupSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button signupButton;
    [SerializeField] private TMP_Text errorText;

    [Header("Navigation")]
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    private BackendApiClient api;

    private void Awake()
    {
        api = new BackendApiClient(BackendConfig.BaseUrl);
        errorText.text = "";
    }

    public void GoToLogin()
    {
        sceneTransitionManager.OnSceneChanged("Sign In");
    }

    public void OnSignupClicked()
    {
        errorText.text = "";

        var user = usernameInput.text.Trim();
        var email = emailInput.text.Trim();
        var pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            errorText.text = "Please fill in all fields.";
            Debug.Log("Please fill in all fields.");
            return;
        }

        if (!email.Contains("@"))
        {
            errorText.text = "Please enter a valid email.";
            Debug.Log("Please enter a valid email.");
            return;
        }

        var payload = new RegisterRequest { username = user, email = email, password = pass };
        StartCoroutine(SignupRoutine(payload));
    }

    private IEnumerator SignupRoutine(RegisterRequest payload)
    {
        SetBusy(true);

        yield return api.RegisterAccount(
            payload,
            onSuccess: (auth) =>
            {
                // token saved by BackendApiClient
                errorText.text = "";
                sceneTransitionManager.OnSceneChanged("Home");
                Debug.Log("token saved to Backend");
            },
            onError: (err) =>
            {
                errorText.text = err;
                Debug.Log(err);
                //errorText.text = err;
            }
        );

        SetBusy(false);
    }

    private void SetBusy(bool busy)
    {
        signupButton.interactable = !busy;
    }
}