using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
#if UNITY_EDITOR
    public bool DebugDisableAudio;
#endif

    //[SerializeField]
    //AK.Wwise.Event StartDayEvent;
    public void StartDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // StartDayEvent.Post(gameObject);
        AkSoundEngine.PostEvent("StartDay", gameObject);
    }

    public void GoToCorkboard()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // StartDayEvent.Post(gameObject);
        AkSoundEngine.PostEvent("MusicCorkboardSfx", gameObject);
    }

    // [SerializeField]
    //AK.Wwise.Event StartNightEvent;
    public void StartNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // StartNightEvent.Post(gameObject);
        AkSoundEngine.PostEvent("StartNight", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event UIClickEvent;
    public void PlayUIClick()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // UIClickEvent.Post(gameObject);
        AkSoundEngine.PostEvent("Click", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event DiscoveryDayEvent;
    public void PlayDiscoveryDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // DiscoveryDayEvent.Post(gameObject);
        AkSoundEngine.PostEvent("DiscoveryDay", gameObject);
    }

   // [SerializeField]
    //AK.Wwise.Event DiscoveryNightEvent;
    public void PlayDiscoveryNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // DiscoveryNightEvent.Post(gameObject);
        AkSoundEngine.PostEvent("DiscoveryNight", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event GameOverDayEvent;
    public void PlayGameOverDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // GameOverDayEvent.Post(gameObject);
        AkSoundEngine.PostEvent("GameOverDay", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event GameOverNightEvent;
    public void PlayGameOverNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //GameOverNightEvent.Post(gameObject);
        AkSoundEngine.PostEvent("GameOverNight", gameObject);
    }

    //[SerializeField]
   // AK.Wwise.Event PaperCrumpleEvent;
    public void PlayPaperCrumple()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // PaperCrumpleEvent.Post(gameObject);
        AkSoundEngine.PostEvent("PaperCrumple", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event PaperMoveEvent;
    public void PlayPaperMove()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //PaperMoveEvent.Post(gameObject);
        AkSoundEngine.PostEvent("PaperMove", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event RoosterCrowEvent;
    public void PlayRoosterCrow()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //RoosterCrowEvent.Post(gameObject);
        AkSoundEngine.PostEvent("RoosterCrow", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event SpeechEvent;
    public void PlaySpeech()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //SpeechEvent.Post(gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event WinDayEvent;
    public void PlayWinDay()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //WinDayEvent.Post(gameObject);
        AkSoundEngine.PostEvent("WinDay", gameObject);
    }

   // [SerializeField]
   // AK.Wwise.Event WinNightEvent;
    public void PlayWinNight()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //WinNightEvent.Post(gameObject);
        AkSoundEngine.PostEvent("WinNight", gameObject);
    }

    //[SerializeField]
    //AK.Wwise.Event WolfHowlEvent;
    public void PlayWolfHowl()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //WolfHowlEvent.Post(gameObject);
        AkSoundEngine.PostEvent("WolfHowl", gameObject);
    }

   // [SerializeField]
    //AK.Wwise.Event BackToTitleEvent;
    public void PlayBackToTitle()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        //BackToTitleEvent.Post(gameObject);
        AkSoundEngine.PostEvent("BackToTitle", gameObject);
    }

   // [SerializeField]
    //AK.Wwise.Event DeathAnnounceEvent;
    public void PlayDeathAnnounce()
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        // DeathAnnounceEvent.Post(gameObject);
        AkSoundEngine.PostEvent("DeathAnnounce", gameObject);
    }

    public void PlaySpeech(int iInstance)
    {
#if UNITY_EDITOR
        if (DebugDisableAudio) return;
#endif
        int iNumberToUse = iInstance + 1;
        string speechName = "Speak_";

        if(iNumberToUse < 10)
        {
            speechName += "0";
        }
        speechName += iNumberToUse.ToString();

        AkSoundEngine.PostEvent(speechName, gameObject);
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
