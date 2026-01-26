using System;
using Newtonsoft.Json;

[Serializable]
public class AuthResponse
{
    [JsonProperty("token")] public string token;
    [JsonProperty("user")] public AuthUser user;
}