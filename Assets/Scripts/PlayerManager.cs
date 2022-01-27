using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<ClueObject> CollectedClues = new List<ClueObject>();
    public SortedDictionary<int, Dictionary<WerewolfGame.TOD, List<ClueObject>>> SortedByDayAndPhaseClues = new SortedDictionary<int, Dictionary<WerewolfGame.TOD, List<ClueObject>>>();

#if UNITY_EDITOR
    public bool DebugGrabSomeClues = false;
#endif

    // Start is called before the first frame update
    void Awake()
    {
        Service.Player = this;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(DebugGrabSomeClues)
        {
            if(Service.Game.CurrentState != WerewolfGame.GameState.PlayerInvestigateDay
                && Service.Game.CurrentState != WerewolfGame.GameState.PlayerInvestigateNight)
            {
                return;
            }

            DebugGrabSomeClues = false;

            if(Service.PhaseSolve.CurrentPhase != null)
            {
                List<int> characterIndices = Randomiser.GetRandomCharacterProcessingOrder();
                int iNumCluesToGrab = UnityEngine.Random.Range(3, 7);

                if (!SortedByDayAndPhaseClues.ContainsKey(Service.Game.CurrentDay))
                {
                    SortedByDayAndPhaseClues.Add(Service.Game.CurrentDay, new Dictionary<WerewolfGame.TOD, List<ClueObject>>());
                    SortedByDayAndPhaseClues[Service.Game.CurrentDay].Add(WerewolfGame.TOD.Day, new List<ClueObject>());
                    SortedByDayAndPhaseClues[Service.Game.CurrentDay].Add(WerewolfGame.TOD.Night, new List<ClueObject>());
                }

                foreach (var index in characterIndices)
                {
                    List<ClueObject> clues = Service.PhaseSolve.CurrentPhase.CharacterCluesToGive[Service.Population.ActiveCharacters[index]];
                    CollectedClues.Add(clues[UnityEngine.Random.Range(0, clues.Count)]);

                    SortedByDayAndPhaseClues[Service.Game.CurrentDay][Service.Game.CurrentTimeOfDay].Add(CollectedClues[CollectedClues.Count - 1]);

                    if (iNumCluesToGrab-- <= 0)
                    {
                        break;
                    }
                }
            }
        }
#endif

    }
}
