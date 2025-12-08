using UnityEngine;

public class BirdManager : MonoBehaviour {
    public GameObject[] points;
    public float birdSpeed;
    public float cooldownTimerAmount;
    private float cooldownTimer;
    private Rigidbody2D rb;
    private int random = 0;
    private bool completed;
    private Vector2 destination;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        completed = true;
        destination = points[0].transform.position;
        //cooldownTimer = 0;
    }

    void Update() {
        if (completed) {
            random = Random.Range(0, points.Length);
            destination = points[random].transform.position;
            completed = false;
        }

        else {
            rb.position = Vector2.MoveTowards(rb.position, destination, Time.deltaTime * birdSpeed);
            if (rb.position == destination) {
                completed = true;
            }
        }
    }
}
