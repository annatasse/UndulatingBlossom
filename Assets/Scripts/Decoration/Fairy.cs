using System.Collections;
using UnityEngine;

public class Fairy : MonoBehaviour {

    // Assign in Inspector
    public Transform[] routes;
    public float speedModifier;
    public Shrine shrine;
    //

    private int nextRoute;
    private float t;
    private Vector2 fairyPosition;
    private bool routeFinished;

    private void Start() {
        nextRoute = 0;
        t = 0;
        routeFinished = true;
    }

    private void Update() {
        if (shrine.unlockable) { return; }
        if (routeFinished) {
            StartCoroutine(MoveAlongRoute(nextRoute));
        }
    }

    private IEnumerator MoveAlongRoute(int route) {
        routeFinished = false;
        Vector2 p0 = routes[route].GetChild(0).position;
        Vector2 p1 = routes[route].GetChild(1).position;
        Vector2 p2 = routes[route].GetChild(2).position;
        Vector2 p3 = routes[route].GetChild(3).position;

        while (t < 1) {
            t += Time.deltaTime * speedModifier;
            fairyPosition = Mathf.Pow(1 - t, 3) * p0 + 3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;
            transform.position = fairyPosition;
            yield return 0;
        }

        t = 0;
        nextRoute++;
        routeFinished = true;
        if (nextRoute == routes.Length) {
            gameObject.SetActive(false);
        }
    }
}
