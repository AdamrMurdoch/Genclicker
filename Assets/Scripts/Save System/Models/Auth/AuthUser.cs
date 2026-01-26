using System;
using Newtonsoft.Json;

[Serializable]
public class AuthUser
{
    [JsonProperty("id")] public string id;
    [JsonProperty("username")] public string username;
    [JsonProperty("email")] public string email;
}