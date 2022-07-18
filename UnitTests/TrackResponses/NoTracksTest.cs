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
        LastfmClient client = new LastfmClient();
        try
        {
            _response = await client.CallAPI(Username, APIKey);
        } catch (LastfmException)
        {
            Fail();
        } catch (HttpRequestException)
        {
            Fail();
        }
    }

    [Test]
    public void NoTrack()
    {
        Track? actualTrack = null;
        Track? respondedTrack = _response?.Track;
        That(actualTrack, Is.EqualTo(respondedTrack));
    }
}