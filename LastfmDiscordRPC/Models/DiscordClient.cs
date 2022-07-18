using System;
using System.Globalization;
using System.IO;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using static System.String;

namespace LastfmDiscordRPC.Models;

public class DiscordClient : IDisposable
{
    private readonly DiscordRpcClient _client;
    private RichPresence? _presence;
    private const string PauseIconURL = @"https://i.imgur.com/AOYINL0.png";
    private const string PlayIconURL = @"https://i.imgur.com/wvTxH0t.png";

    public bool IsInitialised { get; }

    public DiscordClient(string appID)
    {
        if (IsNullOrEmpty(appID)) return;
        _client = new DiscordRpcClient(appID)
        {
            Logger = new FileLoggerTimed(SaveAppData.FolderPath, "RPClog.log", LogLevel.Warning), SkipIdenticalPresence = true
        };
        _client.Initialize();
        IsInitialised = _client.IsInitialized;
    }

    /// <summary>
    /// Sets the discord presence of the user to the response that was received from Last.fm
    /// </summary>
    /// <param name="response">The API response received from last.fm</param>
    /// <param name="username">The username provided by user</param>
    public void SetPresence(LastfmResponse response, string username)
    {
        // Null ignore - handling done in SetPresenceCommand's Execute method.
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
            Label = $"{response.Playcount} scrobbles", Url = @$"https://www.last.fm/user/{username}/"
        };

        _presence = new RichPresence
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
            Buttons = new[]
            {
                button
            }
        };
        _client.SetPresence(_presence);
        _presence = null;

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

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsInitialised)
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

        GC.SuppressFinalize(this);
    }

    ~DiscordClient()
    {
        Dispose();
    }

    private class FileLoggerTimed : ILogger
    {
        private readonly object _fileLock;
        private readonly string _filePath;
        private readonly string _folderPath;
        public LogLevel Level { get; set; }

        public FileLoggerTimed(string folder, string name, LogLevel level)
        {
            Level = level;
            _folderPath = folder;
            _filePath = @$"{folder}\{name}";
            _fileLock = new object();
        }

        private void WriteToFile(string errorLevel, LogLevel level, string message, params object[] args)
        {
            if (Level > level)
                return;
            SaveAppData.CheckFolderExists();
            lock (_fileLock)
            {
                if (File.Exists(_filePath)) File.AppendAllText(_filePath,
                    $"\r\n[{GetCurrentTimeString()}] {errorLevel}: {(args.Length != 0 ? Format(message, args) : message)}");
                else File.WriteAllText(_filePath,
                    $"\r\n[{GetCurrentTimeString()}] {errorLevel}: {(args.Length != 0 ? Format(message, args) : message)}");
            }
        }
        
        private string GetCurrentTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public void Trace(string message, params object[] args) => WriteToFile("TRCE:", LogLevel.Trace, message, args);

        public void Info(string message, params object[] args) => WriteToFile("INFO:", LogLevel.Info, message, args);

        public void Warning(string message, params object[] args) => WriteToFile("WARN:", LogLevel.Warning, message, args);

        public void Error(string message, params object[] args) => WriteToFile("ERR :", LogLevel.Error, message, args);
    }
}