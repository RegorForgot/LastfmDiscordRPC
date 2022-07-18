namespace UnitTests;

public class ResponseStatusTest
{
    private LastfmClient _client = null!;
    private const string Username = "mabdi3";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";
    
    [SetUp]
    public void Setup()
    {
        _client = new LastfmClient();
    }

    [Test]
    public async Task NoError()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            await _client.CallAPI(Username, APIKey);
            Pass();
        } catch (LastfmException)
        {
            Fail();
        } catch (HttpRequestException)
        {
            Fail();
        } 
    }

    [Test]
    public async Task InvalidKey()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            await _client.CallAPI(Username, APIKey);
            Fail();
        } catch (LastfmException e)
        {
            That(e.ErrorCode, Is.EqualTo(LastfmException.ErrorEnum.InvalidKey));
        } catch (HttpRequestException)
        {
            Fail();
        }
    }

    [Test]
    public async Task UserNotExist()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            await _client.CallAPI(Username, APIKey);
            Fail();
        } catch (LastfmException e)
        {
            That(e.ErrorCode, Is.EqualTo(LastfmException.ErrorEnum.InvalidParam));
        } catch (HttpRequestException)
        {
            Fail();
        }
    }

    [Test]
    public async Task NoInternet()
    {
        // Test ran correctly when run in correct conditions.
        try
        {
            await _client.CallAPI(Username, APIKey);
            Fail();
        } catch (LastfmException)
        {
            Fail();
        } catch (HttpRequestException e)
        {
            That(e.StatusCode, 
                Is.EqualTo(Enum.Parse(typeof(HttpStatusCode), "0")));
        }
    }
}