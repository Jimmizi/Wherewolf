using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule
{
    public List<Task> DayTasks = new List<Task>();
    public List<Task> NightTasks = new List<Task>();

    public void Clear()
    {
        DayTasks?.Clear();
        NightTasks?.Clear();
    }
}
