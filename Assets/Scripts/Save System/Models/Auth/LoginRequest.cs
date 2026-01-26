using System;
using Newtonsoft.Json;

[Serializable]
public class LoginRequest
{
    [JsonProperty("username")] public string username;
    [JsonProperty("password")] public string password;
}