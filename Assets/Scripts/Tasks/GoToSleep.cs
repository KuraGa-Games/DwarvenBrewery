using UnityEngine;

public class GoToSleep : MonoBehaviour
{
    [SerializeField] private string taskId = "go_to_sleep";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool isPlayerInRange = false;
    private bool hasSlept = false;

    private void Update()
    {
        if (!hasSlept && isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            TrySleep();
        }
    }

    private void TrySleep()
    {
        if (!TaskManager.Instance.IsTaskAvailable(taskId))
        {
            Debug.Log("ѕока рано спать!");
            return;
        }

        TaskManager.Instance.ReportProgress(taskId);
        hasSlept = true;

        TaskUIManager ui = FindObjectOfType<TaskUIManager>();
        ui.ClearTaskList();

        ScreenFader.Instance.FadeOut(() =>
        {
            if (DayNightCycle.Instance.IsNight)
            {
                DayNightCycle.Instance.StartDay();
            }
            else
            {
                DayNightCycle.Instance.StartNight();
            }

            hasSlept = false; // чтобы можно было снова спать

            ScreenFader.Instance.FadeIn();
        });
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
