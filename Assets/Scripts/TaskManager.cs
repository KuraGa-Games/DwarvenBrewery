using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    public List<TaskData> allTasks = new List<TaskData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TryCompleteTask(string taskID)
    {
        TaskData task = allTasks.Find(t => t.taskID == taskID);
        if (task != null && task.isActive && !task.isCompleted)
        {
            task.isCompleted = true;
            Debug.Log($"Задание выполнено: {task.description}");
            CheckUnlocks();
            CheckAllTasksCompleted();
        }
    }

    private void CheckUnlocks()
    {
        foreach (TaskData task in allTasks)
        {
            if (!task.isActive && !task.isCompleted && !string.IsNullOrEmpty(task.requiredTaskID))
            {
                TaskData required = allTasks.Find(t => t.taskID == task.requiredTaskID);
                if (required != null && required.isCompleted)
                {
                    task.isActive = true;
                    Debug.Log($"Новое задание доступно: {task.description}");
                }
            }
        }
    }

    private void CheckAllTasksCompleted()
    {
        bool allCompleted = true;
        foreach (TaskData task in allTasks)
        {
            if (task.isActive && !task.isCompleted)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            Debug.Log("Все дневные задания выполнены! Можно ложиться спать.");
            // Тут активируем кнопку/задание "Лечь спать"
            ActivateSleepTask();
        }
    }

    private void ActivateSleepTask()
    {
        TaskData sleepTask = allTasks.Find(t => t.taskID == "sleep_day_1");
        if (sleepTask != null)
        {
            sleepTask.isActive = true;
        }
    }

    public bool IsTaskActive(string taskID)
    {
        TaskData task = allTasks.Find(t => t.taskID == taskID);
        return task != null && task.isActive;
    }
}
