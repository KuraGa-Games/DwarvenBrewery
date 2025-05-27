using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TaskUIManager : MonoBehaviour
{
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskListContainer;

    private Dictionary<string, TaskUIItem> activeItems = new Dictionary<string, TaskUIItem>();

    private void Start()
    {
        RefreshTaskList();
    }

    private void Update()
    {
        RefreshTaskList();
    }

    void RefreshTaskList()
    {
        var allTasks = TaskManager.Instance.GetAllTasks();

        foreach (var task in allTasks)
        {
            string id = task.Data.taskID;

            if (!activeItems.ContainsKey(id))
            {
                if (task.IsAvailable || task.IsCompleted)
                {
                    var itemGO = Instantiate(taskItemPrefab, taskListContainer);
                    var item = itemGO.GetComponent<TaskUIItem>();
                    item.Setup(id, task.Data.taskName, task.IsCompleted);
                    activeItems.Add(id, item);
                }
            }
            else
            {
                if (task.IsCompleted)
                    activeItems[id].MarkCompleted();
            }
        }
    }
}
