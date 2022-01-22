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

    public Task(Character owner, TaskType eType)
    {
        TaskOwner = owner;
        Type = eType;

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
    }

    public Character TaskOwner;

    public TaskType Type;
    public Vector3 Position;
    public float Duration;
    public float Timer;

    void SetupWander()
    {
        Duration = UnityEngine.Random.Range(Service.Config.WanderRandomTimeMin, Service.Config.WanderRandomTimeMax);
        // Position = FindWanderPosition(location)
    }

    void SetupIdle()
    {
        Duration = UnityEngine.Random.Range(Service.Config.IdleRandomTimeMin, Service.Config.IdleRandomTimeMax);
        // Position = FindPositionInLocation(location)
    }

    void SetupSleep()
    {
        Duration = -1.0f; // Sleeps the rest of the night
        Position = Service.Population.GetHomePosition(TaskOwner);
    }
    
    void SetupWork()
    {
        Duration = UnityEngine.Random.Range(Service.Config.WorkRandomTimeMin, Service.Config.WorkRandomTimeMax);
        Position = Service.Population.GetWorkPosition(TaskOwner);
    }

}
