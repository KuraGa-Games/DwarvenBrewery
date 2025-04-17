using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListParent;

    private Dictionary<string, TaskData> tasks = new Dictionary<string, TaskData>();
    private Dictionary<string, GameObject> taskUIItems = new Dictionary<string, GameObject>();

    private void Start()
    {
        CreateTasks();
        DrawTaskList();
    }

    private void CreateTasks()
    {
        AddTask(new TaskData("gather_wood", "Добыть 1 охапку дров"));
        AddTask(new TaskData("go_to_river", "Подойти к речке"));
        AddTask(new TaskData("upgrade_brewery", "Улучшить пивоварню", "gather_wood"));
        AddTask(new TaskData("sleep", "Лечь спать", "upgrade_brewery"));
    }

    private void AddTask(TaskData task)
    {
        tasks[task.id] = task;
    }

    private void DrawTaskList()
    {
        foreach (var task in tasks.Values)
        {
            var item = Instantiate(taskItemPrefab, taskListParent);
            var text = item.GetComponentInChildren<Text>();
            text.text = task.description;

            UpdateTaskUIVisual(task.id);
            taskUIItems[task.id] = item;
        }
    }

    public void CompleteTask(string taskId)
    {
        if (!tasks.ContainsKey(taskId)) return;

        tasks[taskId].isCompleted = true;
        UpdateTaskUIVisual(taskId);

        // Открываем все задачи, у которых это было условием
        foreach (var task in tasks.Values)
        {
            if (!task.isAvailable && task.prerequisiteTaskId == taskId)
            {
                task.isAvailable = true;
                UpdateTaskUIVisual(task.id);
            }
        }

        // Проверка — выполнены ли все доступные задания (кроме "sleep")
        if (AllDailyTasksCompleted())
        {
            tasks["sleep"].isAvailable = true;
            UpdateTaskUIVisual("sleep");
        }
    }

    private bool AllDailyTasksCompleted()
    {
        foreach (var task in tasks.Values)
        {
            if (task.id != "sleep" && task.isAvailable && !task.isCompleted)
                return false;
        }
        return true;
    }

    private void UpdateTaskUIVisual(string taskId)
    {
        if (!taskUIItems.ContainsKey(taskId)) return;

        var task = tasks[taskId];
        var item = taskUIItems[taskId];
        var text = item.GetComponentInChildren<Text>();

        var color = Color.white;
        var strike = "";

        if (!task.isAvailable)
        {
            color = new Color(1f, 1f, 1f, 0.5f); // полупрозрачный
        }
        else if (task.isCompleted)
        {
            strike = "<s>"; // зачёркнутый
        }

        text.text = $"{strike}{task.description}{(strike != "" ? "</s>" : "")}";
        text.color = color;
    }
}
