using System;
using Newtonsoft.Json;

[Serializable]
public class RegisterRequest
{
    [JsonProperty("username")] public string username;
    [JsonProperty("email")] public string email;
    [JsonProperty("password")] public string password;
}