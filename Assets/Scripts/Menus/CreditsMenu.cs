using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour {

    // Assign in Inspector
    public RectTransform credits;
    public RectTransform thanks;
    public Button backButton;
    public float scrollSpeed;
    //

    private bool isActive;
    private RectTransform canvas;
    private Vector2 targetPositionCredits;
    private Vector2 targetPositionThanks;
    private float lastUpdate;

    private void Awake() {
        canvas = MenuManager.instance.canvas.GetComponent<RectTransform>();
    }

    public void OnEnable() {
        isActive = true;
        backButton.gameObject.SetActive(false);

        credits.anchoredPosition = Vector2.zero;
        Vector2 offsetThanks = new Vector2(0, credits.sizeDelta.y + canvas.sizeDelta.y / 2);
        thanks.anchoredPosition = credits.anchoredPosition - offsetThanks;
        targetPositionCredits = new Vector2(0, credits.sizeDelta.y + canvas.sizeDelta.y);
        targetPositionThanks = new Vector2(0, canvas.sizeDelta.y / 2);

        lastUpdate = Time.realtimeSinceStartup;
    }

    private void Update() {
        if (isActive) {
            float deltaTime = Time.realtimeSinceStartup - lastUpdate;
            credits.anchoredPosition = Vector2.MoveTowards(credits.anchoredPosition, targetPositionCredits, deltaTime * scrollSpeed);
            thanks.anchoredPosition = Vector2.MoveTowards(thanks.anchoredPosition, targetPositionThanks, deltaTime * scrollSpeed);
            lastUpdate = Time.realtimeSinceStartup;
            if (thanks.anchoredPosition == targetPositionThanks) {
                isActive = false;
                backButton.gameObject.SetActive(true);
                backButton.Select();
            }
        }
    }

    public void Back() {
        isActive = false;
        MenuManager.instance.PreviousMenu();
    }
}
