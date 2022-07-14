using Newtonsoft.Json;

namespace LastfmAPI.Responses;

public class UserResponse : IResponse
{
    public string PlayCount { get; private set; } = null!;
    private UserObject _user = null!;

    [JsonProperty("user")]
    public UserObject User
    {
        get => _user ;
        set
        {
            _user = value;
            PlayCount = _user.PlayCount;
        }
    }
}

public class UserObject
{
    [JsonProperty("playcount")] public string PlayCount { get; set; } = null!;
}