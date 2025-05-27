using UnityEngine;

public enum TaskType { Interact, Collect, ReachPoint, Build, Sleep }

[CreateAssetMenu(menuName = "Task System/Task Data", fileName = "NewTask")]
public class TaskData : ScriptableObject
{
    public string taskID;
    public string taskName;
    [TextArea] public string description;

    public TaskType taskType;
    public string[] prerequisiteTaskIDs;

    public int requiredAmount = 1;

    public bool isNightTask;
}
