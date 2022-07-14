using LastfmAPI.Enum;
using LastfmAPI.Responses;

namespace LastfmAPI.Tests;

public class ResponseStatusTest
{
    private LastfmError response;
    
    [SetUp]
    public void Setup()
    {
        string username = "mabdi36";
        string apiKey = "05467a3191853eb8da38dfb38ed3c73";
        response = (LastfmError) APIConnector.CallAPI(username, apiKey, APIConnector.GetTracks);
    }
    
    [Test]
    public void NoError()
    {
        // Test ran correctly when there was an error and wasn't.
        Assert.That(response.Error, Is.EqualTo(null));
    }

    [Test]
    public void InvalidKey()
    {
        // Test ran correctly when key was incorrect.
        Assert.That(response.Error, Is.EqualTo(ErrorEnum.InvalidKey));
    }
}