using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("MusicCorkboardSfx", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        //AkSoundEngine.StopAll(gameObject);
    }
}
