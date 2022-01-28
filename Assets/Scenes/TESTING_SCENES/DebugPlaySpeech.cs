using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlaySpeech : MonoBehaviour
{
    public bool PlaySpeech = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlaySpeech)
        {
            PlaySpeech = false;
            AkSoundEngine.PostEvent("Speech", gameObject);
        }
    }
}
