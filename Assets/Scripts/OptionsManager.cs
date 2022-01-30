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

    public bool PlayerHasSeenTutorial = false;

    [SerializeField]
    public GameObject OpenTutorialButton;

    private float lastMusicValue = 0.5f;

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

    [SerializeField]
    public GameObject TutorialGameObject;
    public bool ShowingTutorial = false;
    public void CloseTutorial()
    {
        TutorialGameObject?.SetActive(false);
        ShowingTutorial = false;
    }
    public void ShowTutorial()
    {
        ShowingTutorial = true;
        PlayerHasSeenTutorial = true;
        TutorialGameObject?.SetActive(true);

        if (OpenTutorialButton)
        {
            OpenTutorialButton.SetActive(true);
        }
    }

    private static OptionsManager instance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
            Service.Options = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
