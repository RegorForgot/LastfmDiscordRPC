using LastfmAPI.Enums;
using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
using static LastfmAPI.APIConnector;

namespace LastfmAPI.Tests;

public class UserTest
{
    // All tests completed at the time they were completed
    private UserResponse _response = null!;
    private const string Username = "mabdi3";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";

    [SetUp]
    public void Setup()
    { }

    [Test]
    public async Task CorrectPlaycount()
    {
        try
        {
            _response = (UserResponse) await CallAPI(Username, APIKey, GetUser);
            Assert.That(_response.PlayCount, Is.EqualTo("59109"));
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
            _response = (UserResponse) await CallAPI(Username, APIKey, GetUser); 
            Assert.Fail();
        } catch (LastfmException e)
        {
            Assert.That(e.ErrorCode, Is.EqualTo(ErrorEnum.InvalidParam));
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }
}
