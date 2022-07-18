namespace UnitTests;

public class UserTest
{
    // All tests completed at the time they were completed
    private LastfmClient _client = null!;
    private LastfmResponse? _response;
    private const string Username = "mabdi3";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";

    [SetUp]
    public void Setup()
    {
        _client = new LastfmClient();
    }
    
    [Test]
    public async Task CorrectPlaycount()
    {
        try
        {
            _response = await _client.CallAPI(Username, APIKey);
            That(_response?.Playcount, Is.EqualTo("59109"));
        } catch (LastfmException)
        {
            Fail();
        } catch (HttpRequestException)
        {
            Fail();
        }
    }
    
    [Test]
    public async Task IncorrectUser()
    {
        try
        {
            _response = await _client.CallAPI(Username, APIKey); 
            Fail();
        } catch (LastfmException e)
        {
            That(e.ErrorCode, Is.EqualTo(LastfmException.ErrorEnum.InvalidParam));
        } catch (HttpRequestException)
        {
            Fail();
        }
    }
}
