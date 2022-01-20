using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameFSM = FSM<GameManager, float>;
using GameStateBase = FSM<GameManager, float>.StateBase<GameManager>;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    bool skipStart;

    [SerializeField]
    GameObject StartEntity;
    [SerializeField]
    GameObject PlayingEntity;
    [SerializeField]
    GameObject GameOverEntity;
    [SerializeField]
    GameObject GameWonEntity;

    [SerializeField]
    string ProceedActionName = "Submit";

    public int WinStreak { get; private set; }

    private enum States {Start, Playing, Won, GameOver};
    private GameFSM m_fsm = new GameFSM();

    public void Restart()
    {
        m_fsm.SetState((int) States.Playing);
    }
    public void WinGame()
    {
        ++WinStreak;
        m_fsm.SetState((int) States.Won);
    }

    public void LoseGame()
    {
        WinStreak = 0;
        m_fsm.SetState((int) States.GameOver);
    }
    private void Awake()
    {
        var startState = m_fsm.AddState<StartState>((int) States.Start, this);
        if(startState != null && skipStart)
        {
            startState.SetSkipInput();
        }

        m_fsm.AddState<PlayingState>((int) States.Playing, this);
        m_fsm.AddState<GameOverState>((int) States.GameOver, this);
        m_fsm.AddState<GameWonState>((int) States.Won, this);
    }

    void Start()
    {
        m_fsm.SetState((int) States.Start);
    }

    void Update()
    {
        m_fsm.Process(Time.deltaTime);
    }

    private class StartState : GameStateBase
    {
        private bool skipInput = false;

        public void SetSkipInput() => skipInput = true;

        public override void OnEnter()
        {
            Owner.StartEntity?.SetActive(true);
            Owner.PlayingEntity?.SetActive(false);
            Owner.GameOverEntity?.SetActive(false);
            Owner.GameWonEntity?.SetActive(false);
        }
        public override void Process(float deltaTime)
        {
            if (Input.GetButtonUp(Owner.ProceedActionName) || skipInput)
            {
                Continue((int) States.Playing);
            }
        }
    }

    private class PlayingState : GameStateBase
    {
        public override void OnEnter()
        {
            Owner.StartEntity?.SetActive(false);
            Owner.PlayingEntity?.SetActive(true);
            Owner.GameOverEntity?.SetActive(false);
            Owner.GameWonEntity?.SetActive(false);
        }
    }

    private class GameWonState : GameStateBase
    {
        public override void OnEnter()
        {
            Owner.StartEntity?.SetActive(false);
            Owner.PlayingEntity?.SetActive(false);
            Owner.GameOverEntity?.SetActive(false);
            Owner.GameWonEntity?.SetActive(true);
        }
    }
    
    private class GameOverState : GameStateBase
    {
        public override void OnEnter()
        {
            Owner.StartEntity?.SetActive(false);
            Owner.PlayingEntity?.SetActive(false);
            Owner.GameOverEntity?.SetActive(true);
            Owner.GameWonEntity?.SetActive(false);
        }
    }
}
