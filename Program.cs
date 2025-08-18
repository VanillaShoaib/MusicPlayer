using System;
using System.IO;
using NAudio.Wave;

IWavePlayer? waveOutDevice = null;
AudioFileReader? audioFile = null;
bool isPlaying = false;
string currentSong = "";

Console.WriteLine("🎵🎵Welcome to My Music Player!🎵🎵");
Console.WriteLine("======================================");
Console.WriteLine("Enter A Path to a Directory containing Music Or Leave it Empty and Press to use your Current Directory.");

string musicFolder = Console.ReadLine();
if (string.IsNullOrEmpty(musicFolder))
{
    musicFolder = Directory.GetCurrentDirectory();
}

if (!Directory.Exists(musicFolder))
{
    Console.WriteLine("❌Error 404:\nDirectory not exist!❌");
    musicFolder = Directory.GetCurrentDirectory();
}

ShowMusicFiles(musicFolder);
ShowMenu();
while (true) {
    Console.WriteLine("\nEnter Your Choice: ");
    string input = Console.ReadLine().ToLower();

    switch (input)
    {
        case "1" or "play":
            PlayMusic(musicFolder);
            break;

        case "2" or "pause":
            PauseMusic();
            break;

        case "3" or "stop":
            StopMusic();
            break;

        case "4" or "volume":
            ChangeVolume();
            break;

        case "5" or "list":
            ShowMusicFiles(musicFolder);
            break;

        case "6" or "help":
            ShowMenu();
            break;

        case "7" or "quit" or "exit":
            StopMusic();
            Console.WriteLine("ByeBye 👋\nThanks for using Me!\n~https://github.com/VanillaShoaib");
            return;

        default:
            Console.WriteLine("❌Invalid! Type 'help' to see options.❌");
            break;
    }
}

void ShowMenu()
{
    Console.WriteLine("\n Music Player Menu:");
    Console.WriteLine("1. Play a Song");
    Console.WriteLine("2. Pause/Resume");
    Console.WriteLine("3. Stop");
    Console.WriteLine("4. Change Volume");
    Console.WriteLine("5. List Songs");
    Console.WriteLine("6. Show Menu");
    Console.WriteLine("7. Quit");
    Console.WriteLine("(💡 You can type number or commands like 'play', 'pause', etc.)");
}

static void ShowMusicFiles(string folder)
{
    Console.WriteLine($"\n These are the Music Files in: {folder}");
    Console.WriteLine("=============================================");

    string[] musicFiles = Directory.GetFiles(folder, "*.mp3");

    if (musicFiles.Length == 0)
    {
        Console.WriteLine($"❌Error 404:\nNo MP3 Files available in {folder}❌");
        Console.WriteLine("💡You May Put a MP3 File or Change the Directory");
        return;
    }

    for (int i = 0; i < musicFiles.Length; i++)
    {
        string fileName = Path.GetFileName(musicFiles[i]);
        Console.WriteLine($"{i + 1}. {fileName}");
    }
}

void PlayMusic(string folder)
{
    string[] musicFiles = Directory.GetFiles(folder, "*.mp3");

    if (musicFiles.Length == 0)
    {
        Console.WriteLine("❌No MP3 Files Found!❌");
        return;
    }

    if (isPlaying && waveOutDevice != null)
    {
        if (waveOutDevice.PlaybackState == PlaybackState.Paused)
        {
            waveOutDevice.Play();
            Console.WriteLine($"▶️ Resumed: {currentSong}");
            return;
        }
        else if (waveOutDevice.PlaybackState == PlaybackState.Playing)
        {
            Console.WriteLine($"🎵Already Playing: {currentSong}🎵");
            return;
        }
    }

    Console.WriteLine("Enter Song to Play:");
    for (int i = 0; i < musicFiles.Length; i++)
    {
        Console.WriteLine($"{i + 1}. {Path.GetFileName(musicFiles[i])}");
    }

    string? input = Console.ReadLine();
    if (int.TryParse(input, out int songNumber) && songNumber > 0 && songNumber <= musicFiles.Length)
    {
        try
        {
            StopMusic();

            string selectedFile = musicFiles[songNumber - 1];
            currentSong = Path.GetFileName(selectedFile);

            audioFile = new AudioFileReader(selectedFile);
            waveOutDevice = new WaveOutEvent();
            waveOutDevice.Init(audioFile);
            waveOutDevice.Play();
            isPlaying = true;

            Console.WriteLine($"Now Playing: {currentSong}");
            Console.WriteLine("Use 'pause' to Pause, 'stop' to Stop");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌Error playing file: {ex.Message}❌");
        }
    }
    else
    {
        Console.WriteLine("❌Invalid Song Number!❌");
    }
}

void PauseMusic()
{
    if (waveOutDevice != null && isPlaying)
    {
        if (waveOutDevice.PlaybackState == PlaybackState.Playing)
        {
            waveOutDevice.Pause();
            Console.WriteLine($"⏸️ Paused: {currentSong}");
        }
        else if (waveOutDevice.PlaybackState == PlaybackState.Paused)
        {
            waveOutDevice.Play();
            Console.WriteLine($"▶️ Resumed: {currentSong}");
        }
    }
    else
    {
        Console.WriteLine("❌ No music is currently playing!");
    }
}

void StopMusic()
{
    if (waveOutDevice != null)
    {
        waveOutDevice.Stop();
        waveOutDevice.Dispose();
        waveOutDevice = null;
    }

    if (audioFile != null)
    {
        audioFile.Dispose();
        audioFile = null;
    }

    isPlaying = false;

    if (!string.IsNullOrEmpty(currentSong))
    {
        Console.WriteLine($"⏹️ Stopped: {currentSong}");
        currentSong = "";
    }
}

void ChangeVolume()
{
    if (waveOutDevice != null && isPlaying)
    {
        Console.WriteLine("Enter volume (0-100):");
        string? input = Console.ReadLine();
        
        if (int.TryParse(input, out int volume) && volume >= 0 && volume <= 100)
        {
            waveOutDevice.Volume = volume / 100.0f;
            Console.WriteLine($"🔊 Volume set to: {volume}%");
        }
        else
        {
            Console.WriteLine("❌ Invalid volume! Enter a number between 0-100");
        }
    }
    else
    {
        Console.WriteLine("❌ No music is currently playing!");
    }
}