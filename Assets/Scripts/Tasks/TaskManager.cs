using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<TaskData> allTasks;
    private Dictionary<string, TaskStatus> taskStatuses = new Dictionary<string, TaskStatus>();

    public static TaskManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        InitializeTasks();
    }

    private void InitializeTasks()
    {
        foreach (var task in allTasks)
        {
            taskStatuses[task.taskID] = new TaskStatus(task);
        }

        UpdateTaskAvailability();
    }

    private void UpdateTaskAvailability()
    {
        foreach (var task in allTasks)
        {
            var status = taskStatuses[task.taskID];

            if (status.IsCompleted) continue;

            if (task.prerequisiteTaskIDs.Length == 0 || task.prerequisiteTaskIDs.All(id => taskStatuses[id].IsCompleted))
            {
                status.IsAvailable = true;
            }
        }
    }

    public void ReportProgress(string taskID, int amount = 1)
    {
        if (!taskStatuses.ContainsKey(taskID)) return;

        var status = taskStatuses[taskID];

        if (!status.IsAvailable || status.IsCompleted) return;

        status.CurrentAmount += amount;

        if (status.CurrentAmount >= status.Data.requiredAmount)
        {
            status.IsCompleted = true;
            Debug.Log($"Задание выполнено: {status.Data.taskName}");

            UpdateTaskAvailability();
        }
    }

    public bool IsTaskCompleted(string taskID)
    {
        return taskStatuses.ContainsKey(taskID) && taskStatuses[taskID].IsCompleted;
    }

    public bool IsTaskAvailable(string taskID)
    {
        return taskStatuses.ContainsKey(taskID) && taskStatuses[taskID].IsAvailable;
    }

    public IEnumerable<TaskStatus> GetAvailableTasks()
    {
        bool isNight = DayNightCycle.Instance.IsNight;
        return taskStatuses.Values
            .Where(t => t.IsAvailable && !t.IsCompleted && t.Data.isNightTask == isNight);
    }


    public IEnumerable<TaskStatus> GetAllTasks()
    {
        return taskStatuses.Values;
    }
}

[Serializable]
public class TaskStatus
{
    public TaskData Data;
    public bool IsAvailable = false;
    public bool IsCompleted = false;
    public int CurrentAmount = 0;

    public TaskStatus(TaskData data)
    {
        Data = data;
    }
}

