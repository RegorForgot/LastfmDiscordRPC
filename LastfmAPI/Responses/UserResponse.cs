using Newtonsoft.Json;

namespace LastfmAPI.Responses;

public class UserResponse : IResponse
{
    public string PlayCount { get; set; }
    private User _user;

    [JsonProperty("user")]
    public User user
    {
        get => _user ;
        set
        {
            _user = value;
            PlayCount = _user.playCount;
        }
    }
    
    public class User
    {
        [JsonProperty("playcount")] public string playCount { get; set; }
    }
}