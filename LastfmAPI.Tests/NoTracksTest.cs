using LastfmAPI.Responses;

namespace LastfmAPI.Tests;

public class NoTracksTest
{
    // All tests completed at the time they were completed
    private TrackResponse _response;

    [SetUp]
    public void Setup()
    {
        string username = "aaaaaaaaaaaaaa";
        string apiKey = "05467a3191853eb8da38dfb38ed3c733";
        _response = (TrackResponse)APIConnector.CallAPI(username, apiKey, APIConnector.GetTracks);
    }

    [Test]
    public void NoTrack()
    {
        Track? actualTrack = null;
        Track? respondedTrack = _response.Track;
        Assert.That(actualTrack, Is.EqualTo(respondedTrack));
    }
}