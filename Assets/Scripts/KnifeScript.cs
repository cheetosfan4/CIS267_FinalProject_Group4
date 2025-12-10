using UnityEngine;

public class KnifeScript : MonoBehaviour {
    private bool collided;
    private EnemyManager enemy;
    private GameObject enemyCollider;

    private void Awake() {
        collided = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collided = true;
            enemyCollider = collision.gameObject;
            enemy = enemyCollider.GetComponent<EnemyManager>();
        }
    }

    public bool hasCollided() {
        return collided;
    }
    public void setCollided(bool b) {
        collided = b;
    }
    public EnemyManager getEnemy() {
        return enemy;
    }
    public GameObject getEnemyCollider() {
        return enemyCollider;
    }
}
