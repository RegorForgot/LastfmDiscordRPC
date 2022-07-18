namespace UnitTests.TrackResponses;

public class NoTracksTest
{
    // All tests completed at the time they were completed
    private LastfmResponse? _response;
    private const string Username = "aaaaaaaaaaaaaa";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";

    [SetUp]
    public async Task Setup()
    {
        try
        {
            _response = await CallAPI(Username, APIKey);
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
        Track? respondedTrack = _response?.Track;
        Assert.That(actualTrack, Is.EqualTo(respondedTrack));
    }
}