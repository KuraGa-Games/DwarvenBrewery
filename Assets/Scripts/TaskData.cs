using System;

[Serializable]
public class TaskData
{
    public string taskID;                  // Уникальный ID задания
    public string description;             // Описание задания
    public bool isCompleted = false;       // Выполнено ли
    public bool isActive = false;          // Доступно ли в данный момент
    public string requiredTaskID;          // ID задания, которое нужно выполнить, чтобы это стало доступным
}
