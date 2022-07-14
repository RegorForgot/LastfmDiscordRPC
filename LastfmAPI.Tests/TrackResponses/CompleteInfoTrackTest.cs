using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
using static LastfmAPI.APIConnector;

namespace LastfmAPI.Tests.TrackResponses;

public class CompleteInfoTrackTest
{
    // All tests completed at the time they were completed
    private TrackResponse _response = null!;
    private const string Username = "mabdi36";
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
    public void Name()
    {
        const string actualTrack = "bye bye cubase";
        string? respondedTrackName = _response.Track?.Name;
        Assert.That(respondedTrackName, Is.EqualTo(actualTrack));
    }

    [Test]
    public void Artist()
    {
        const string actualArtist = "鈴木ねんど";
        string? respondedTrackArtist = _response.Track?.Artist.Name;
        Assert.That(respondedTrackArtist, Is.EqualTo(actualArtist));
    }

    [Test]
    public void Album()
    {
        const string actualArtist = "鈴木ねんどのアーメン粘土細工EP";
        string? respondedTrackArtist = _response.Track?.Album.Name;
        Assert.That(respondedTrackArtist, Is.EqualTo(actualArtist));
    }

    [Test]
    public void NowPlaying()
    {
        const string actualNowPlaying = "true";
        string? respondedNowPlaying = _response.Track?.NowPlaying?.State;
        Assert.That(respondedNowPlaying, Is.EqualTo(actualNowPlaying));
    }

    [Test]
    public void Timestamp()
    {
        string? actualTimestamp = null;
        string? respondedTimestamp = _response.Track?.Date?.Timestamp;
        Assert.That(actualTimestamp, Is.EqualTo(respondedTimestamp));
    }

    [Test]
    public void Image()
    {
        const string actualImage = @"https://lastfm.freetls.fastly.net/i/u/174s/25877ecfb2e656c3d76d0af36ddae5b5.jpg";
        string? respondedImage = _response.Track?.ImageURL;
        Assert.That(respondedImage, Is.EqualTo(actualImage));
    }
}