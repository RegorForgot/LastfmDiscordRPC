# Last.fm Discord Rich Presence
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/e0da00a962b5448cbd888887b9f9a77f)](https://www.codacy.com/gh/RegorForgotTheirPassword/LastfmDiscordRPC/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=RegorForgotTheirPassword/LastfmDiscordRPC&amp;utm_campaign=Badge_Grade)

![Presence](https://raw.githubusercontent.com/RegorForgotTheirPassword/LastfmDiscordRPC/master/Screenshots/Presence.png)

![Program](https://raw.githubusercontent.com/RegorForgotTheirPassword/LastfmDiscordRPC/master/Screenshots/Program.png)

A simple Windows-only* application that sets your Discord presence to your last played track on Last.fm!

## Usage

Simply download the release of your choice, enter your username, and set the presence!

**Note:** For the non-self contained releases **(recommended)**, .NET 6 Desktop Runtime must be installed on your computer

[Download .NET 6 x64 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.7-windows-x64-installer)


## Customisation

Thanks to [curiositIy](https://github.com/curiositIy/) for writing the section below!

### **Setting your own Last.fm API key**

You can use the default Last.fm API key by simply clicking "Load defaults", but if any problems (such as rate-limiting) arise, you can always use yor own.

Head over to [last.fm/api/account/create](https://www.last.fm/api/account/create) and create an API account.

Don't worry about making it serious, simply name it "Discord RPC" or similar and then click **Submit**. You don't need to give callback URL or application homepage.

Now copy and paste the **API Key** given to you into the program.

### **Making your own Discord app and ID**

Head over to [discord.com/developers/applications](https://discord.com/developers/applications) and click the button that says **New application** in the top right.

Name it whatever you'd like, but this name will be the one that will be used on the presence's title:

![discord app name example](https://raw.githubusercontent.com/RegorForgotTheirPassword/LastfmDiscordRPC/master/Screenshots/PresenceName.png)

Once you create the application you should be brought the application page, if not look for it in the menu and click on it.

Finally, look for the text that says **Application ID**, there will be an ID below that, either copy that directly or click the **Copy** button. You can now paste this into the program!

## Contributing

If you find any issues or have any suggestions, feel free to start an issue or create a pull request!

## Libraries and assets

- [Github Logo](https://github-media-downloads.s3.amazonaws.com/GitHub-Mark.zip)
- [Last.fm Logo](https://www.last.fm/static/images/lastfm_avatar_twitter.png)
- [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp/)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [RestSharp](https://github.com/restsharp/RestSharp)
- [WpfAnimatedGif](https://github.com/XamlAnimatedGif/WpfAnimatedGif/)
- [InnoSetup](https://github.com/jrsoftware/issrc)
