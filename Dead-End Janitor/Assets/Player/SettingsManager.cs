using System;
public class SettingsManager
{
    // Singleton instance
    private static SettingsManager _instance;

    // Lock for thread safety
    private static readonly object _lock = new object();

    // Private constructor to prevent instantiation
    private SettingsManager() { }

    // Public access point to the instance
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SettingsManager();
                    }
                }
            }
            return _instance;
        }
    }

    // Settings properties
    public float MasterVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 0.8f;
    public float SFXVolume { get; set; } = 0.8f;
    public bool Fullscreen { get; set; } = true;
    public int ScreenResolutionWidth { get; set; } = 1920;
    public int ScreenResolutionHeight { get; set; } = 1080;

    // Save and Load Settings (example placeholders)
    public void SaveSettings()
    {
        // Save settings to PlayerPrefs or a file
        // PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        // PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        // PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        // PlayerPrefs.SetInt("Fullscreen", Fullscreen ? 1 : 0);
        // PlayerPrefs.SetInt("ResolutionWidth", ScreenResolutionWidth);
        // PlayerPrefs.SetInt("ResolutionHeight", ScreenResolutionHeight);
        // PlayerPrefs.Save();

        // UnityEngine.Debug.Log("Settings saved.");
    }

    public void LoadSettings()
    {
        // Load settings from PlayerPrefs or a file
        // MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        // MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        // SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        // Fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        // ScreenResolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        // ScreenResolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);

        // UnityEngine.Debug.Log("Settings loaded.");
    }
}
