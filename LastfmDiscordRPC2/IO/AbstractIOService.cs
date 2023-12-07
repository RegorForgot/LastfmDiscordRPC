using System;
using System.IO;
using static LastfmDiscordRPC2.Utilities.URIOpen;
using OperatingSystem = LastfmDiscordRPC2.Utilities.OperatingSystem;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace LastfmDiscordRPC2.IO;

public abstract class AbstractIOService
{
    public virtual string FilePath { get; protected set; }
    
    protected static readonly string SaveFolder;
    protected static readonly object FileLock;

    static AbstractIOService()
    {
        SaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        SaveFolder += OperatingSystem.CurrentOS switch
        {
            DataTypes.OperatingSystem.Windows => @"/AppData/Local/LastfmDiscordRPC",
            DataTypes.OperatingSystem.Linux => @"/.LastfmDiscordRPC",
            DataTypes.OperatingSystem.OSX => @"/Library/Application Support/LastfmDiscordRPC",
            _ => ""
        };
        
        CreateDataDirectoryIfNotExist();
        FileLock = new object();
    }
    
    protected void WriteToFile(string msg)
    {
        lock (FileLock)
        {
            File.WriteAllText(FilePath, msg);
        }
    }

    protected void AppendToFile(string msg)
    {
        lock (FileLock)
        {
            File.AppendAllText(FilePath, msg);
        }
    }

    protected bool FileExists()
    {
        lock (FileLock)
        {
            return File.Exists(FilePath);
        }
    }

    private static void CreateDataDirectoryIfNotExist()
    {
        if (!Directory.Exists(SaveFolder))
        {
            Directory.CreateDirectory(SaveFolder);
        }
    }

    public static void OpenFolder()
    {
        OpenURI(SaveFolder);
    }
}