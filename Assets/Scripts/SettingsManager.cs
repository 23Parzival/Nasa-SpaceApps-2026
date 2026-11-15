using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string FullscreenKey = "Fullscreen";

    private const float DefaultMasterVolume = 1f;
    private const float DefaultMusicVolume = 1f;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    //raised when the music volume changes, so a music player can update its AudioSource
    public static event Action<float> MusicVolumeChanged;

    public static float MasterVolume
    {
        get { return PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume); }
        set
        {
            value = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
            AudioListener.volume = value;
        }
    }

    public static float MusicVolume
    {
        get { return PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume); }
        set
        {
            value = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
            MusicVolumeChanged?.Invoke(value);
        }
    }

    public static bool Fullscreen
    {
        get { return PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            Screen.fullScreen = value;
        }
    }

    //applies the saved settings when the game starts, even if no SettingsManager is in the scene
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedSettings()
    {
        AudioListener.volume = MasterVolume;
        if (PlayerPrefs.HasKey(FullscreenKey))
        {
            Screen.fullScreen = Fullscreen;
        }
    }

    void OnEnable()
    {
        //show the saved values when settings panel is pulled up
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(MasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.SetValueWithoutNotify(MusicVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(Fullscreen);
        }
    }

    void OnDisable()
    {
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
    }

    public void SetFullscreen(bool value)
    {
        Fullscreen = value;
    }
}
