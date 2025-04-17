using System;
using UnityEngine;

[Serializable]
public class TaskData
{
    public string id;                      // Уникальный ID задания
    public string description;             // Описание задания
    public bool isCompleted;               // Завершено ли задание
    public bool isAvailable;               // Доступно ли задание
    public string prerequisiteTaskId;      // ID задания, после выполнения которого станет доступным

    public TaskData(string id, string description, string prerequisiteTaskId = "")
    {
        this.id = id;
        this.description = description;
        this.prerequisiteTaskId = prerequisiteTaskId;
        this.isCompleted = false;
        this.isAvailable = string.IsNullOrEmpty(prerequisiteTaskId); // Если нет условия, доступно сразу
    }
}
