using Unity.VisualScripting;
using UnityEngine;

public class ArtifactBurstScript : MonoBehaviour {
    public float destroyTimer;
    private bool ready;
    private Rigidbody2D rb;
    private GameObject lastEnemyHit;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        ready = false;
    }
    void Update() {
        if (ready) {
            destroyTimer -= Time.deltaTime;
        }
        if (destroyTimer <= 0) {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("DestructibleTough")) {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject != lastEnemyHit)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerManager>().hitEnemy(collision.gameObject.GetComponent<EnemyManager>(), collision.gameObject);
            lastEnemyHit = collision.gameObject;

            if (lastEnemyHit.name == "Final_Boss")
            {
                FinalBossScript boss = collision.gameObject.GetComponent<FinalBossScript>();
                if (boss.shieldUp)
                {
                    boss.shieldBreak();
                }
                return;
            }
        }

    }

    public void setReady() {
        ready = true;
    }
}
