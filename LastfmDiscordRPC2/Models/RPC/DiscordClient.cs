// using System;
// using System.Globalization;
// using System.Text;
// using DiscordRPC;
// using DiscordRPC.Exceptions;
// using LastfmDiscordRPC2.IO;
// using LastfmDiscordRPC2.IO.Schema;
// using LastfmDiscordRPC2.Logging;
// using LastfmDiscordRPC2.Models.Responses;
//
// namespace LastfmDiscordRPC2.Models.RPC;
//
// public class DiscordClient : IDisposable
// {
//     private DiscordRpcClient? _client;
//     private readonly AbstractConfigFileIO<SaveData> _saveData;
//     private readonly IRPCLogger _logger;
//
//     private const string PauseIconURL = "https://i.imgur.com/AOYINL0.png";
//     private const string PlayIconURL = "https://i.imgur.com/wvTxH0t.png";
//
//     private bool IsInitialised
//     {
//         get => _client != null;
//     }
//     
//     public DiscordClient(AbstractConfigFileIO<SaveData> saveData, IRPCLogger logger)
//     {
//         _saveData = saveData;
//         _logger = logger;
//     }
//     
//     public bool IsReady { get; private set; }
//     public long LastScrobbleTime { get; private set; }
//
//     public void Initialize()
//     {
//         if (IsInitialised)
//         {
//             return;
//         }
//
//         _client = new DiscordRpcClient(_saveData.ConfigData.AppID);
//         _client.Initialize();
//         _client.Logger = _logger;
//
//         SetEventHandlers();
//     }
//
//     private void SetEventHandlers()
//     {
//         if (_client == null)
//         {
//             return;
//         }
//
//         _client.OnClose += (_, _) =>
//         {
//             _logger.Error("Could not connect to Discord.");
//             IsReady = false;
//         };
//         _client.OnError += (_, _) =>
//         {
//             _logger.Error("There has been an error trying to connect to Discord.");
//             IsReady = false;
//         };
//         _client.OnConnectionFailed += (_, _) =>
//         {
//             _logger.Error("Connection to discord failed. Check if your Discord app is open.");
//             IsReady = false;
//         };
//         _client.OnConnectionEstablished += (_, _) =>
//         {
//             _logger.Info("Connection to discord succeeded.");
//         };
//         _client.OnReady += (_, _) =>
//         {
//             _logger.Info("Client ready.");
//             IsReady = true;
//         };
//     }
//
//     /// <summary>
//     /// Sets the discord presence of the user to the response that was received from Last.fm
//     /// </summary>
//     /// <param name="response">The API response received from last.fm</param>
//     /// <param name="username">The username provided by user</param>
//     public void SetPresence(TrackResponse response, string username)
//     {
//         Track track = response.Track;
//         string smallImage;
//         string smallText;
//         string albumName = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
//
//         if (response.Track.NowPlaying.State == "true")
//         {
//             smallImage = PlayIconURL;
//             smallText = "Now playing";
//             LastScrobbleTime = DateTimeOffset.Now.ToUnixTimeSeconds();
//         }
//         else
//         {
//             smallImage = PauseIconURL;
//
//             if (long.TryParse(response.Track.Date.Timestamp, NumberStyles.Number, null, out long lastScrobbleTime))
//             {
//                 LastScrobbleTime = lastScrobbleTime;
//                 TimeSpan timeSince = TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - LastScrobbleTime);
//                 smallText = $"Last played {GetTimeString(timeSince)}";
//             }
//             else
//             {
//                 smallText = "Stopped.";
//             }
//         }
//
//         Button button = new Button
//         {
//             Label = $"{int.Parse(response.PlayCount):n0} scrobbles", Url = $"https://www.last.fm/user/{username}/"
//         };
//
//         RichPresence presence = new RichPresence
//         {
//             Details = GetUTF8String(track.Name),
//             State = GetUTF8String($"By {track.Artist.Name}{albumName}"),
//             Assets = new DiscordRPC.Assets
//             {
//                 LargeImageKey = track.Images[3].URL,
//                 LargeImageText = $"{(IsNullOrEmpty(track.Album.Name) ? null : track.Album.Name)}",
//                 SmallImageKey = smallImage,
//                 SmallImageText = smallText
//             },
//             Buttons = new[]
//             {
//                 button
//             }
//         };
//         _client?.SetPresence(presence);
//
//         string GetTimeString(TimeSpan timeSince)
//         {
//             string days = timeSince.Days == 0 ? "" : $"{timeSince.Days}d ";
//             string hours = timeSince.Hours == 0 ? "" : $"{timeSince.Hours % 24}h ";
//             string minutes = timeSince.Minutes == 0 ? "" : $"{timeSince.Minutes % 60}m ";
//             string seconds = timeSince.Seconds == 0 ? "" : $"{timeSince.Seconds % 60}s";
//
//             return $"{days}{hours}{minutes}{seconds}";
//         }
//
//         string GetUTF8String(string input)
//         {
//             if (input.Length < 2)
//             {
//                 input += "\u180E";
//             }
//
//             if (Encoding.UTF8.GetByteCount(input) <= 128)
//             {
//                 return input;
//             }
//
//             byte[] buffer = new byte[128];
//             char[] inputChars = input.ToCharArray();
//             Encoding.UTF8.GetEncoder().Convert(inputChars, 0, inputChars.Length, buffer, 0, buffer.Length,
//                 false, out _, out int bytesUsed, out _);
//
//             return Encoding.UTF8.GetString(buffer, 0, bytesUsed);
//         }
//     }
//
//     private void ClearPresence()
//     {
//         _client?.ClearPresence();
//     }
//
//     /// <inheritdoc />
//     public void Dispose()
//     {
//         if (!IsInitialised || (bool)_client?.IsDisposed)
//         {
//             return;
//         }
//
//         try
//         {
//             ClearPresence();
//             _client.Deinitialize();
//             _client.Dispose();
//         }
//         catch (Exception ex) when (ex is ObjectDisposedException or UninitializedException)
//         {
//             // If either of these are run, the client did not need to be disposed in the first place.
//         }
//         finally
//         {
//             GC.SuppressFinalize(this);
//         }
//     }
// }