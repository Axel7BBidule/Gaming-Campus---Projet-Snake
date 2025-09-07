using Raylib_CsLo;

public class SongManager
{
    public const string DefaultAppleSoundPath = "song/apple.wav";
    public const string DefaultThemeMusicPath = "song/theme.mp3";
    public const string DefaultClickSoundPath = "song/click.wav";

    private Sound? appleSound;
    private Sound? clickSound;
    private Music? themeMusic;
    private bool appleLoaded;
    private bool clickLoaded;
    private bool musicLoaded;
    private bool themePlaying = false;

    public string AppleSoundPath { get; private set; }
    public string ClickSoundPath { get; private set; }
    public string ThemeMusicPath { get; private set; }

    public SongManager(string appleSoundPath, string clickSoundPath, string themeMusicPath)
    {
        AppleSoundPath = appleSoundPath;
        ClickSoundPath = clickSoundPath;
        ThemeMusicPath = themeMusicPath;
        LoadAppleSound();
        LoadClickSound();
        LoadThemeMusic();
    }

    private void LoadAppleSound()
    {
        if (appleLoaded && appleSound.HasValue)
        {
            Raylib.UnloadSound(appleSound.Value);
            appleLoaded = false;
        }
        if (Raylib.FileExists(AppleSoundPath))
        {
            appleSound = Raylib.LoadSound(AppleSoundPath);
            appleLoaded = true;
        }
    }

    private void LoadClickSound()
    {
        if (clickLoaded && clickSound.HasValue)
        {
            Raylib.UnloadSound(clickSound.Value);
            clickLoaded = false;
        }
        if (Raylib.FileExists(ClickSoundPath))
        {
            clickSound = Raylib.LoadSound(ClickSoundPath);
            clickLoaded = true;
        }
    }

    private void LoadThemeMusic()
    {
        if (musicLoaded && themeMusic.HasValue)
        {
            Raylib.UnloadMusicStream(themeMusic.Value);
            musicLoaded = false;
        }
        if (Raylib.FileExists(ThemeMusicPath))
        {
            themeMusic = Raylib.LoadMusicStream(ThemeMusicPath);
            musicLoaded = true;
        }
    }

    public void SetAppleSoundPath(string newPath)
    {
        AppleSoundPath = newPath;
        LoadAppleSound();
    }

    public void SetClickSoundPath(string newPath)
    {
        ClickSoundPath = newPath;
        LoadClickSound();
    }

    public void SetThemeMusicPath(string newPath)
    {
        ThemeMusicPath = newPath;
        LoadThemeMusic();
    }

    public void PlayAppleSound()
    {
        if (appleLoaded && appleSound.HasValue)
            Raylib.PlaySound(appleSound.Value);
    }

    public void PlayClickSound()
    {
        if (clickLoaded && clickSound.HasValue)
            Raylib.PlaySound(clickSound.Value);
    }

    public void PlayTheme()
    {
        if (musicLoaded && themeMusic.HasValue && !themePlaying)
        {
            Raylib.PlayMusicStream(themeMusic.Value);
            themePlaying = true;
        }
    }

    public void StopTheme()
    {
        if (musicLoaded && themeMusic.HasValue && themePlaying)
        {
            Raylib.StopMusicStream(themeMusic.Value);
            themePlaying = false;
        }
    }

    public void UpdateTheme()
    {
        if (musicLoaded && themeMusic.HasValue && themePlaying)
            Raylib.UpdateMusicStream(themeMusic.Value);
    }

    public void SetThemeVolume(float volume)
    {
        if (musicLoaded && themeMusic.HasValue)
            Raylib.SetMusicVolume(themeMusic.Value, volume);
    }

    public void Unload()
    {
        if (musicLoaded && themeMusic.HasValue)
        {
            Raylib.UnloadMusicStream(themeMusic.Value);
            musicLoaded = false;
            themePlaying = false; // Pour éviter un état incohérent
        }
        if (appleLoaded && appleSound.HasValue)
        {
            Raylib.UnloadSound(appleSound.Value);
            appleLoaded = false;
        }
        if (clickLoaded && clickSound.HasValue)
        {
            Raylib.UnloadSound(clickSound.Value);
            clickLoaded = false;
        }
    }

    public Music? ThemeMusic => themeMusic;
    public bool IsThemeLoaded => musicLoaded;

    ~SongManager()
    {
        Unload();
    }
}