namespace UnitTests;

public class UserTest
{
    // All tests completed at the time they were completed
    private LastfmResponse _response;
    private const string Username = "mabdi3";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";
    
    [Test]
    public async Task CorrectPlaycount()
    {
        try
        {
            _response = await CallAPI(Username, APIKey);
            Assert.That(_response.UserInfo.Playcount, Is.EqualTo("59109"));
        } catch (LastfmException)
        {
            Assert.Fail();
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }
    
    [Test]
    public async Task IncorrectUser()
    {
        try
        {
            _response = await CallAPI(Username, APIKey); 
            Assert.Fail();
        } catch (LastfmException e)
        {
            Assert.That(e.ErrorCode, Is.EqualTo(LastfmException.ErrorEnum.InvalidParam));
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }
}
