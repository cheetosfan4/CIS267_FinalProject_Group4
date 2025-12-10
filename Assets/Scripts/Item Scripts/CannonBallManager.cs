using Unity.VisualScripting;
using UnityEngine;

public class CannonBallManager : MonoBehaviour {
    public float ballSpeed;
    public float destroyTimer;
    private bool ready;
    private Vector2 direction;
    private Rigidbody2D rb;
    private GameObject lastEnemyHit;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        ready = false;
        direction = Vector2.zero;
    }
    void Update() {
        if (ready) {
            rb.linearVelocity = new Vector2(ballSpeed * direction.x, ballSpeed * direction.y);
            destroyTimer -= Time.deltaTime;
        }
        if (destroyTimer <= 0) {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Destructible")) {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject != lastEnemyHit) {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerManager>().hitEnemy(collision.gameObject.GetComponent<EnemyManager>(), collision.gameObject);
            lastEnemyHit = collision.gameObject;
        }
    }

    public void setReady() {
        ready = true;
    }
    public void setDirection(Vector2 d) {
        direction = d;
    }
}
