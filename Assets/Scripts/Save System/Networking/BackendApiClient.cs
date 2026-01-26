using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class BackendApiClient
{
    private readonly string baseUrl;

    public BackendApiClient(string baseUrl)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
    }

    private const string TokenKey = "TheTokenIsAlwaysWatching";

    public string Token => PlayerPrefs.GetString(TokenKey, "");

    public void SetToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            PlayerPrefs.DeleteKey(TokenKey);
        }
        else
        {
            PlayerPrefs.SetString(TokenKey, token);
        }
        PlayerPrefs.Save();
    }

    private UnityWebRequest CreateRequest(string url, string method, string jsonBody = null)
    {
        var req = new UnityWebRequest(url, method);
        req.downloadHandler = new DownloadHandlerBuffer();

        if (!string.IsNullOrEmpty(jsonBody))
        {
            var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.SetRequestHeader("Content-Type", "application/json");
        }

        if (!string.IsNullOrEmpty(Token))
        {
            req.SetRequestHeader("Authorization", $"Bearer {Token}");
        }

        return req;
    }

    public IEnumerator RegisterAccount(RegisterRequest payload, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
    {
        string json = JsonConvert.SerializeObject(payload);

        using (var req = CreateRequest($"{baseUrl}/api/auth/register", "POST", json))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"{req.responseCode}: {req.error} | {req.downloadHandler.text}");
                yield break;
            }

            AuthResponse response;
            try 
            { 
                response = JsonConvert.DeserializeObject<AuthResponse>(req.downloadHandler.text);     
            }
            catch (System.Exception e)
            {
                onError?.Invoke($"Register parse failed: {e.Message}");
                yield break;
            }

            if (string.IsNullOrEmpty(response?.token))
            {
                onError?.Invoke($"Register was successful but the token is missing.");
                yield break;
            }

            SetToken(response.token);
            onSuccess?.Invoke(response);
        }
    }

    public IEnumerator Login(LoginRequest payload, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
    {
        string json = JsonConvert.SerializeObject(payload);

        using (var req = CreateRequest($"{baseUrl}/api/auth/login", "POST", json))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"{req.responseCode}: {req.error} | {req.downloadHandler.text}");
                yield break;
            }

            AuthResponse response;
            try 
            { 
                response = JsonConvert.DeserializeObject<AuthResponse>(req.downloadHandler.text); 
            }
            catch (System.Exception e)
            {
                onError?.Invoke($"Login JSON parse failed: {e.Message}");
                yield break;
            }

            if (string.IsNullOrEmpty(response?.token))
            {
                onError?.Invoke($"Login has succeeded but the token is missing!");
                yield break;
            }

            SetToken(response.token);
            onSuccess?.Invoke(response);
        }
    }

    public IEnumerator GetUser(System.Action<AuthUser> onSuccess, System.Action<string> onError)
    {
        using (var req = CreateRequest($"{baseUrl}/api/auth/me", "GET"))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"{req.responseCode}: {req.error} | {req.downloadHandler.text}");
                yield break;
            }

            AuthUser user;
            try 
            { 
                user = JsonConvert.DeserializeObject<AuthUser>(req.downloadHandler.text); 
            }
            catch (System.Exception e)
            {
                onError?.Invoke($"GetMe JSON parse failed: {e.Message}");
                yield break;
            }

            onSuccess?.Invoke(user);
        }
    }

    public IEnumerator GetSave(System.Action<SavePayload> onSuccess, System.Action<string> onError)
    {
        using (var req = CreateRequest($"{baseUrl}/api/save", "GET"))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"{req.responseCode}: {req.error} | {req.downloadHandler.text}");
                yield break;
            }

            var response = JsonConvert.DeserializeObject<SaveResponse>(req.downloadHandler.text);
            onSuccess?.Invoke(response?.data);
        }
    }

    public IEnumerator PutSave(SavePayload payload, System.Action onSuccess, System.Action<string> onError)
    {
        string json = JsonConvert.SerializeObject(payload);

        using (var req = CreateRequest($"{baseUrl}/api/save", "PUT", json))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"{req.responseCode}: {req.error} | {req.downloadHandler.text}");
                yield break;
            }

            onSuccess?.Invoke();
        }
    }
}