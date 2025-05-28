using UnityEngine;

public class GoToSleep : MonoBehaviour
{
    [SerializeField] private string dayTaskId = "go_to_sleep";
    [SerializeField] private string nightTaskId = "go_to_sleep_night";

    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Звук сна")]
    [SerializeField] private AudioClip sleepSound;
    [SerializeField] private float volume = 1f;

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
        string currentTaskId = DayNightCycle.Instance.IsNight ? nightTaskId : dayTaskId;

        if (!TaskManager.Instance.IsTaskAvailable(currentTaskId))
        {
            Debug.Log("Пока рано спать!");
            return;
        }

        TaskManager.Instance.ReportProgress(currentTaskId);
        hasSlept = true;

        // Звук сна
        if (sleepSound != null)
        {
            AudioSource.PlayClipAtPoint(sleepSound, transform.position, volume);
        }

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

            hasSlept = false;
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
