using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    public enum TaskType
    {
        WanderArea,
        Idle,
        Sleep,
        Work
    }

    public static TaskType InvalidTaskType => (TaskType)(-1);

    public Task(Character owner, TaskType eType, int iLoc = -1)
    {
        TaskOwner = owner;
        Type = eType;
        Location = iLoc;

        // Setup the tasks
        switch (eType)
        {
            case TaskType.WanderArea:
                SetupWander();
                break;
            case TaskType.Idle:
                SetupIdle();
                break;
            case TaskType.Sleep:
                SetupSleep();
                break;
            case TaskType.Work:
                SetupWork();
                break;
        }

        UpdatePosition();
    }

    public Character TaskOwner;

    public TaskType Type;
    public Vector3 Position;
    public int Location = -1;
    public float Duration;
    public float Timer;

    public bool ShouldFinish => Duration > 0.0f && Timer >= Duration;

    public static TaskType GetRandomTaskType(TaskType eExcludeType = (TaskType)(-1), bool bDayTask = true)
    {
        List<TaskType> eTaskTypes = new List<TaskType>();

        eTaskTypes.Add(TaskType.WanderArea);
        eTaskTypes.Add(TaskType.Idle);
        eTaskTypes.Add(bDayTask ? TaskType.Work : TaskType.Sleep);

        if (eExcludeType != InvalidTaskType)
        {
            eTaskTypes.Remove(eExcludeType);
        }

        return eTaskTypes[UnityEngine.Random.Range(0, eTaskTypes.Count)];
    }

    public void CalculateWanderDuration()
    {
        Duration = UnityEngine.Random.Range(Service.Config.WanderRandomTimeMin, Service.Config.WanderRandomTimeMax);
    }

    public void CalculateIdleDuration()
    {
        Duration = UnityEngine.Random.Range(Service.Config.IdleRandomTimeMin, Service.Config.IdleRandomTimeMax);
    }

    public void CalculateWorkDuration()
    {
        Duration = UnityEngine.Random.Range(Service.Config.WorkRandomTimeMin, Service.Config.WorkRandomTimeMax);
    }

    public static int GetRandomLocation() => UnityEngine.Random.Range(Emote.LocationMin, Emote.LocationMax + 1);

    public void UpdatePosition()
    {
        if(Location == -1 || Location < Emote.LocationMin || Location > Emote.LocationMax)
        {
            Location = GetRandomLocation();
        }

        switch (Type)
        {
            case TaskType.Work:
                Service.Population.GetWorkPositionAndLocation(TaskOwner, out Position, out Location);
                Service.Population.PhysicalCharacterMap[TaskOwner].CurrentDestination = Position;
                break;
            case TaskType.WanderArea:
            case TaskType.Idle:
                Position = Service.Location.GetRandomNavmeshPositionInLocation(Location);
                Service.Population.PhysicalCharacterMap[TaskOwner].CurrentDestination = Position;
                break;
        }

        
    }

    public void WarpToTaskPosition()
    {
        if (Position != Vector3.zero)
        {
            Service.Population.PhysicalCharacterMap[TaskOwner].gameObject.transform.position = Position;
        }
    }

    void SetupWander()
    {
        
    }

    void SetupIdle()
    {
        
    }

    void SetupSleep()
    {
        Duration = -1.0f; // Sleeps the rest of the night
        Position = Service.Population.GetHomePosition(TaskOwner);
        Location = Service.Population.GetHomeLocation(TaskOwner);
    }
    
    void SetupWork()
    {
        
    }

    public void Update()
    {
        Timer += Time.deltaTime;

        // TODO Do tasking stuffs
    }
}
