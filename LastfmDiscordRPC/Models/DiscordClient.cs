using System;
using System.Globalization;
using DiscordRPC;
using DiscordRPC.Logging;
using static System.String;

namespace LastfmDiscordRPC.Models;

public class DiscordClient : IDisposable
{
    private readonly DiscordRpcClient _client;
    private const string PauseIconURL = @"https://i.imgur.com/AOYINL0.png";
    private const string PlayIconURL =  @"https://i.imgur.com/wvTxH0t.png";
    
    public DiscordClient(string appID)
    {
        _client = new DiscordRpcClient(appID)
        {
            Logger = new FileLoggerTimed($@"{SaveAppData.FolderPath}\RPClog.log", LogLevel.Info),
            SkipIdenticalPresence = true
        };
        _client.Initialize();
    }

    /// <summary>
    /// Sets the discord presence of the user to the response that was received from Last.fm
    /// </summary>
    /// <param name="response">The API response received from last.fm</param>
    /// <param name="username">The username provided by user</param>
    public void SetPresence(LastfmResponse response, string username)
    {
        // Null ignore - handling done in ActivateCommand's Execute method.
        Track track = response.Track!;
        string smallImage;
        string smallText;
        string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
        
        if (response.Track!.NowPlaying.State == "true")
        {
            smallImage = PlayIconURL;
            smallText = "Now playing";
        } else
        {
            smallImage = PauseIconURL;

            if (long.TryParse(response.Track.Date.Timestamp, NumberStyles.Number, null, out long unixTimeStamp))
            {
                TimeSpan timeSince = TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - unixTimeStamp);
                smallText = $"Last played {GetTimeString(timeSince)}";
            } else
            {
                smallText = "Stopped.";
            }
        }

        Button button = new Button
        {
            Label = $"{response.Playcount} scrobbles", 
            Url = @$"https://www.last.fm/user/{username}/"
        };
        
        RichPresence presence = new RichPresence
        {
            Details = track.Name,
            State = $"By {track.Artist.Name}{albumString}",
            Assets = new Assets
            {
                LargeImageKey = track.Images[3].URL,
                LargeImageText = $"{response.Playcount} scrobbles",
                SmallImageKey = smallImage,
                SmallImageText = smallText
            },
            Buttons = new [] {button}
        };
        _client.SetPresence(presence);

        string GetTimeString(TimeSpan timeSince)
        {
            string days = timeSince.Days == 0 ? "" : $"{timeSince.Days}d ";
            string hours = timeSince.Hours == 0 ? "" : $"{timeSince.Hours % 24}h ";
            string minutes = timeSince.Minutes == 0 ? "" : $"{timeSince.Minutes % 60}m ";
            string seconds = timeSince.Seconds == 0 ? "" : $"{timeSince.Seconds % 60}s";

            return $"{days}{hours}{minutes}{seconds}";
        }
    }

    public bool HasPresence()
    {
        return _client.CurrentPresence != null;
    }

    public void ClearPresence()
    {
        if (HasPresence()) _client.ClearPresence();  
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        if (_client.IsDisposed) return;
            
        try
        {
            _client.ClearPresence();
            _client.Deinitialize();
            _client.Dispose();
        } catch (ObjectDisposedException)
        { }
    }

    public class FileLoggerTimed : ILogger
    {
        private readonly object _fileLock;
        public LogLevel Level { get; set; }
        public string File { get; set; }
        
        
        public FileLoggerTimed(string path, LogLevel level)
        {
            Level = level;
            File = path;
            _fileLock = new object();
        }

        private string GetCurrentTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace)
                return;
            lock (_fileLock)
                System.IO.File.AppendAllText(File, 
                $"\r\n[{GetCurrentTimeString()}] TRCE: " + (args.Length != 0 ? Format(message, args) : message));
        }

        public void Info(string message, params object[] args)
        {
            if (Level > LogLevel.Info)
                return;
            lock (_fileLock)
                System.IO.File.AppendAllText(File, 
                $"\r\n[{GetCurrentTimeString()}] INFO: " + (args.Length != 0 ? Format(message, args) : message));
        }

        public void Warning(string message, params object[] args)
        {
            if (Level > LogLevel.Warning)
                return;
            lock (_fileLock)
                System.IO.File.AppendAllText(File, 
                $"\r\n[{GetCurrentTimeString()}] WARN: " + (args.Length != 0 ? Format(message, args) : message));
        }

        public void Error(string message, params object[] args)
        {
            if (Level > LogLevel.Error)
                return;
            lock (_fileLock)
                System.IO.File.AppendAllText(File, 
                $"\r\n[{GetCurrentTimeString()}] ERR : " + (args.Length != 0 ? Format(message, args) : message));
        }

    }
}