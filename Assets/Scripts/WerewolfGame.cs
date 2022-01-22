using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfGame : GameManager
{
    public enum TOD
    {
        Day,
        Night
    }

#if UNITY_EDITOR
    public bool DebugGoToNextPhase;
#endif


    public TOD TimeOfDay = TOD.Day;

    protected override void Awake()
    {
        Service.Game = this;

        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        if(DebugGoToNextPhase)
        {
            DebugGoToNextPhase = false;

            if(TimeOfDay == TOD.Day)
            {
                TimeOfDay = TOD.Night;
            }
            else if (TimeOfDay == TOD.Night)
            {
                TimeOfDay = TOD.Day;
            }

            ProcessPhaseShiftOnAllCharacters();
        }
#endif
    }

    void ProcessPhaseShiftOnAllCharacters()
    {
        foreach(var c in Service.Population.ActiveCharacters)
        {
            c.OnTimeOfDayPhaseShift();
        }
    }

}
