using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class FSM<Owner, ProcessParam>
{
    public const int InvalidID = -1;
    public abstract class StateBase<SubOwner> where SubOwner : Owner
    {
        public virtual void OnEnter() {}
        public virtual void Process(ProcessParam param) {}
        public virtual void OnLeave() {}

        public SubOwner Owner
        {
            get { return m_owner;}
            set { if (m_owner == null) { m_owner = value;}}
        }

        public FSM<Owner, ProcessParam> Master
        {
            get { return  m_master; }
            set { if(m_master == null) { m_master = value; } }
        }

        public void Continue(int nextStage)
        {
            m_master.SetState(nextStage);
        }

        private SubOwner m_owner;
        private FSM<Owner, ProcessParam> m_master;
    }

    public int CurrentStateID
    {
        get {return m_currentStateId; }
    }
    
    public StateBase<Owner> CurrentState
    {
        get { return m_currentState; }
    }

    public void SetState(int stateID)
    {
        if (stateID < InvalidID)
        {
            return;
        }

        if (m_processLock)
        {
            m_nextStateId = stateID;
        }
        else
        {
            ChangeState(stateID);
        }
    }

    public void Process(ProcessParam param)
    {
        m_processLock = true;
        if (m_currentState != null)
        {
            m_currentState.Process(param);
        }

        if (m_nextStateId != m_currentStateId)
        {
            ChangeState(m_nextStateId);
        }

        m_processLock = false;
    }

    private void ChangeState(int stateId)
    {
        if (stateId == m_currentStateId)
        {
            return;
        }

        m_nextStateId = stateId;
        if (m_currentState != null)
        {
            m_currentState.OnLeave();
        }

        if (stateId <= InvalidID)
        {
            m_currentState = null;
        }
        else
        {
            m_currentState = m_states[stateId];
            m_currentState.OnEnter();
        }

        m_currentStateId = stateId;
    }

    public void AddState<State>(int stateId, Owner owner) where State : StateBase<Owner>, new()
    {
        if (stateId > InvalidID)
        {
            State created = new State();
            created.Owner = owner;
            created.Master = this;
            m_states[stateId] = created;
        }
    }

    public State GetCachedStateAs<State>(int statedId) where State: StateBase<Owner>
    {
        return m_states[statedId] as State;
    }

    private Dictionary<int, StateBase<Owner>> m_states = new Dictionary<int, StateBase<Owner>>();
    private int m_currentStateId = InvalidID;
    private int m_nextStateId = InvalidID;
    private bool m_processLock = false;
    private StateBase<Owner> m_currentState = null;
}