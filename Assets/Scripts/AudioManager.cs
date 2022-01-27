using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public AK.Wwise.Event StartDayEvent;

    [SerializeField]
    public AK.Wwise.Event StartNightEvent;

    [SerializeField]
    public List<AK.Wwise.Event> MusicEvents;

    [SerializeField]
    public List<AK.Wwise.Event> SfxEvents;

    public void StartDay()
    {
        StartDayEvent.Post(gameObject);
    }

    public void StartNight()
    {
        StartNightEvent.Post(gameObject);
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
