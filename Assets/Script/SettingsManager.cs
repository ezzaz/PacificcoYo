using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;


    private void Start()
    {
        
            if (PlayerPrefs.HasKey( "musicVolume"))
            {
                LoadVolume();
            }
            else
            {
                SetMusicVolume();
            }

        
    }
    public void SetMusicVolume()
    {
        float volumen = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("musicVolume", volumen);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }
}
