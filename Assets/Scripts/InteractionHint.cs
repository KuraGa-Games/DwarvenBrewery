using UnityEngine;

public class InteractionHint : MonoBehaviour
{
    public static InteractionHint Instance;

    [SerializeField] private GameObject hintUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (hintUI != null) hintUI.SetActive(false);
    }

    public void ShowHint()
    {
        if (hintUI != null) hintUI.SetActive(true);
    }

    public void HideHint()
    {
        if (hintUI != null) hintUI.SetActive(false);
    }
}
