using System.Net;
using LastfmAPI.Enums;
using LastfmAPI.Exceptions;
using static LastfmAPI.APIConnector;

namespace LastfmAPI.Tests;

public class ResponseStatusTest
{
    private const string Username = "mabdi3";
    private const string APIKey = "05467a3191853eb8da38dfb38ed3c733";
    
    [SetUp]
    public void Setup() 
    { }

    [Test]
    public void NoError()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            CallAPI(Username, APIKey, GetTracks);
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
    public void InvalidKey()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            CallAPI(Username, APIKey, GetTracks);
            Assert.Fail();
        } catch (LastfmException e)
        {
            Assert.That(e.ErrorCode, Is.EqualTo(ErrorEnum.InvalidKey));
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void UserNotExist()
    {
        // Test ran correctly when ran with correct parameters.
        try
        {
            CallAPI(Username, APIKey, GetTracks);
            Assert.Fail();
        } catch (LastfmException e)
        {
            Assert.That(e.ErrorCode, Is.EqualTo(ErrorEnum.InvalidParam));
        } catch (HttpRequestException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void NoInternet()
    {
        // Test ran correctly when run in correct conditions.
        try
        {
            CallAPI(Username, APIKey, GetTracks);
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