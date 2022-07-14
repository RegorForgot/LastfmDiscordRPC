using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
using static LastfmAPI.APIConnector;

namespace LastfmAPI.Tests.TrackResponses;

public class NoTracksTest
{
    // All tests completed at the time they were completed
    private TrackResponse _response = null!;
    private const string Username = "aaaaaaaaaaaaaa";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";

    [SetUp]
    public void Setup()
    {
        try
        {
            _response = (TrackResponse) CallAPI(Username, APIKey, GetTracks);
        } catch (LastfmException)
        {
            Assert.Fail();
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void NoTrack()
    {
        Track? actualTrack = null;
        Track? respondedTrack = _response.Track;
        Assert.That(actualTrack, Is.EqualTo(respondedTrack));
    }
}