using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
#if UNITY_EDITOR
    public bool DebugDisableAudio;
#endif

    [SerializeField]
    AK.Wwise.Event StartDayEvent;
    public void StartDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        StartDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event StartNightEvent;
    public void StartNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        StartNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event UIClickEvent;
    public void PlayUIClick()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        UIClickEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event DiscoveryDayEvent;
    public void PlayDiscoveryDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        DiscoveryDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event DiscoveryNightEvent;
    public void PlayDiscoveryNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        DiscoveryNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event GameOverDayEvent;
    public void PlayGameOverDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        GameOverDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event GameOverNightEvent;
    public void PlayGameOverNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        GameOverNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event PaperCrumpleEvent;
    public void PlayPaperCrumple()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        PaperCrumpleEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event PaperMoveEvent;
    public void PlayPaperMove()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        PaperMoveEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event RoosterCrowEvent;
    public void PlayRoosterCrow()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        RoosterCrowEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event SpeechEvent;
    public void PlaySpeech()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        SpeechEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event WinDayEvent;
    public void PlayWinDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        WinDayEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event WinNightEvent;
    public void PlayWinNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        WinNightEvent.Post(gameObject);
    }

    [SerializeField]
    AK.Wwise.Event WolfHowlEvent;
    public void PlayWolfHowl()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
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
