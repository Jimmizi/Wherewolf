using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    public GameObject ClosedOptions;

    [SerializeField]
    public GameObject OpenOptions;

    [SerializeField]
    public Slider MusicSlider;

    private float lastMusicValue = 0.75f;

    public void PlayClickSound()
    {
        if(Service.Transition != null)
        {
            Service.Transition.PlayClickSound();
        }
        else if (Service.Audio != null)
        {
            Service.Audio.PlayUIClick();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(MusicSlider)
        {
            Service.MusicVolume = MusicSlider.value;
            if(Service.MusicVolume != lastMusicValue)
            {
                Debug.Log("Setting master_volume to " + Service.MusicVolume * 100.0f);
                AkSoundEngine.SetRTPCValue("master_volume", Service.MusicVolume * 100.0f);
            }

            lastMusicValue = Service.MusicVolume;
        }
    }
}