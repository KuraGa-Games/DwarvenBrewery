using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    private string taskID;

    public void Setup(string id, string text, bool isCompleted)
    {
        taskID = id;
        taskText.text = text;
        taskText.fontStyle = isCompleted ? FontStyles.Strikethrough : FontStyles.Normal;
    }

    public void MarkCompleted()
    {
        taskText.fontStyle = FontStyles.Strikethrough;
    }

    public string GetTaskID() => taskID;
}
