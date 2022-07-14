using LastfmAPI.Responses;

namespace LastfmAPI.Tests;

public class NoAlbumTrackTest
{
    private TrackResponse _response;

    [SetUp]
    public void Setup()
    {
        string username = "mabdi36";
        string apiKey = "05467a3191853eb8da38dfb38ed3c733";
        _response = (TrackResponse)APIConnector.CallAPI(username, apiKey, APIConnector.GetTracks);
    }

    [Test]
    public void Name()
    {
        string actualTrack = "Wicked (Confidence)";
        string? respondedTrackName = _response.Track?.Name;
        Assert.That(actualTrack, Is.EqualTo(respondedTrackName));
    }

    [Test]
    public void Artist()
    {
        string actualArtist = "Rems";
        string? respondedTrackArtist = _response.Track?.Artist.Name;
        Assert.That(actualArtist, Is.EqualTo(respondedTrackArtist));
    }

    [Test]
    public void Album()
    {
        string? actualArtist = null;
        string? respondedTrackArtist = _response.Track?.Album.Name;
        Assert.That(actualArtist, Is.EqualTo(respondedTrackArtist));
    }

    [Test]
    public void NowPlaying()
    {
        string actualNowPlaying = "true";
        string? respondedNowPlaying = _response.Track?.NowPlaying?.State;
        Assert.That(actualNowPlaying, Is.EqualTo(respondedNowPlaying));
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
        string? actualImage = null;
        string? respondedImage = _response.Track?.ImageURL;
        Assert.That(actualImage, Is.EqualTo(respondedImage));
    }
}