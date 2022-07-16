namespace UnitTests;

public class ResponseStatusTest
{
    private const string Username = "mabdi3";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";
    
    [SetUp]
    public void Setup() 
    { }

    [Test]
    public async Task NoError()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            await CallAPI(Username, APIKey);
            Assert.Pass();
        } catch (LastfmException)
        {
            Assert.Fail();
        } catch (HttpRequestException)
        {
            Assert.Fail();
        } 
    }

    [Test]
    public async Task InvalidKey()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            await CallAPI(Username, APIKey);
            Assert.Fail();
        } catch (LastfmException e)
        {
            Assert.That(e.ErrorCode, Is.EqualTo(LastfmException.ErrorEnum.InvalidKey));
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public async Task UserNotExist()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            await CallAPI(Username, APIKey);
            Assert.Fail();
        } catch (LastfmException e)
        {
            Assert.That(e.ErrorCode, Is.EqualTo(LastfmException.ErrorEnum.InvalidParam));
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public async Task NoInternet()
    {
        // Test ran correctly when run in correct conditions.
        try
        {
            await CallAPI(Username, APIKey);
            Assert.Fail();
        } catch (LastfmException)
        {
            Assert.Fail();
        } catch (HttpRequestException e)
        {
            Assert.That(e.StatusCode, 
                Is.EqualTo(Enum.Parse(typeof(HttpStatusCode), "0")));
        }
    }
}