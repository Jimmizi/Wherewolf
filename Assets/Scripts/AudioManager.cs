using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AK.Wwise.Event StartDayEvent;
    public void StartDay()
    {
        StartDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event StartNightEvent;
    public void StartNight()
    {
        StartNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event UIClickEvent;
    public void PlayUIClick()
    {
        UIClickEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event DiscoveryDayEvent;
    public void PlayDiscoveryDay()
    {
        DiscoveryDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event DiscoveryNightEvent;
    public void PlayDiscoveryNight()
    {
        DiscoveryNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event GameOverDayEvent;
    public void PlayGameOverDay()
    {
        GameOverDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event GameOverNightEvent;
    public void PlayGameOverNight()
    {
        GameOverNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event PaperCrumpleEvent;
    public void PlayPaperCrumple()
    {
        PaperCrumpleEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event PaperMoveEvent;
    public void PlayPaperMove()
    {
        PaperMoveEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event RoosterCrowEvent;
    public void PlayRoosterCrow()
    {
        RoosterCrowEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event SpeechEvent;
    public void PlaySpeech()
    {
        SpeechEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event WinDayEvent;
    public void PlayWinDay()
    {
        WinDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event WinNightEvent;
    public void PlayWinNight()
    {
        WinNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event WolfHowlEvent;
    public void PlayWolfHowl()
    {
        WolfHowlEvent.Post(gameObject);
    }





    // Start is called before the first frame update
    void Awake()
    {
        Service.Audio = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
