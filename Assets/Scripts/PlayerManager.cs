using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public GameObject knifePrefab;
    public float knifeStabLength;
    public float moveSpeed;
    public Sprite[] sprites;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float inputHorizontal;
    private float inputVertical;
    private Vector2 currentRoom;
    private bool canMove;
    private int currentDirection;
    private float knifeTimer;
    private GameObject myKnife;
    private GameObject lastChestTouched;
    private MusicManager musicPlayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        currentRoom = Vector2.zero;
        canMove = true;
        currentDirection = 0;
        knifeTimer = 0;
        musicPlayer = GameManager.instance.gameObject.GetComponentInChildren<MusicManager>();
    }

    void Update() {
        movePlayer();
        attack();
        openChest();

        if (knifeTimer > 0) {
            knifeTimer -= Time.deltaTime;
        }
    }

    private void movePlayer() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        if (canMove) {
            rb.linearVelocity = new Vector2(moveSpeed * inputHorizontal, moveSpeed * inputVertical);
            flipPlayer();
        }
    }

    private void flipPlayer() {
        if (Time.timeScale != 0) {
            int direction = 99;
            //right
            if (inputHorizontal > 0) {
                direction = 3;
            }
            //left
            else if (inputHorizontal < 0) {
                direction = 2;
            }
            //separate if statement for vertical flipping,
            //so the front and back sprites take priority over the side sprites
            //may change how this works in the future

            //back
            if (inputVertical > 0) {
                direction = 1;
            }
            //front
            else if (inputVertical < 0) {
                direction = 0;
            }

            //only changes direction if any of the axes are greater than zero,
            //so the player doesnt flip when not moving
            if (direction != 99) {
                sr.sprite = sprites[direction];
                currentDirection = direction;
            }
        }
    }

    private void attack() {
        if (Time.timeScale != 0) {
            if (Input.GetKeyDown(KeyCode.F) && knifeTimer <= 0 && myKnife == null && canMove) {
                float newX = 0;
                float newY = 0;
                Vector3 rotation = Vector3.zero;
                knifeTimer = knifeStabLength;
                myKnife = Instantiate(knifePrefab);
                myKnife.transform.position = this.gameObject.transform.position;
                
                switch (currentDirection) {
                    //front
                    case 0: {
                            newY = -0.75f;
                            rotation.z = -90;
                            break;
                        }
                    //back
                    case 1: {
                            newY = 0.75f;
                            rotation.z = 90;
                            break;
                        }
                    //left
                    case 2: {
                            newX = -0.75f;
                            rotation.z = 180;
                            break;
                        }
                    //right
                    case 3: {
                            newX = 0.75f;
                            break;
                        }
                }

                myKnife.transform.position += new Vector3(newX, newY, 0);
                myKnife.transform.eulerAngles = rotation;
                canMove = false;
            }
            else if (myKnife != null && myKnife.GetComponent<KnifeScript>().hasCollided() && knifeTimer > 0) {
                if (myKnife.GetComponent<KnifeScript>().hasCollided()) {
                    GameObject enemyCollider = myKnife.GetComponent<KnifeScript>().getEnemyCollider();
                    EnemyManager enemy = enemyCollider.GetComponent<EnemyManager>();

                    hitEnemy(enemy, enemyCollider);
                    //Debug.Log("hit enemy");
                    myKnife.GetComponent<KnifeScript>().setCollided(false);
                }
            }
            else if (myKnife != null && knifeTimer <= 0) {
                Destroy(myKnife);
                canMove = true;
            }
        }
    }

    private void openChest() {
        if (Input.GetKeyUp(KeyCode.Q) && lastChestTouched != null) {
            float distanceBetween = Vector2.Distance(lastChestTouched.transform.position, rb.position);
            Debug.Log("calculated chest distance");

            if (distanceBetween <= 2) {
                lastChestTouched.GetComponent<ChestManager>().open();
                Debug.Log("opened chest");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("RoomPresence")) {
            //each room has a "room_presence" object that has a trigger collider
            //when the player enters a new room, the location of the room is sent to the camera
            GameObject camera = GameObject.FindWithTag("MainCamera");
            Vector3 roomCoordinates = Vector3.zero;
            roomCoordinates.x = collision.gameObject.transform.position.x;
            roomCoordinates.y = collision.gameObject.transform.position.y;
            currentRoom = roomCoordinates;

            if (camera != null) {
                camera.GetComponent<CameraManager>().moveToRoom(roomCoordinates);
            }
            else {
                Debug.Log("camera not found");
            }
        }
        if (collision.CompareTag("GateToTwo")) {
            SceneManager.LoadScene("LevelTwo");
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
            currentDirection = 0;
        }
        if (collision.CompareTag("GateToThree")) {
            SceneManager.LoadScene("LevelThree");
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
            currentDirection = 0;
        }
        if (collision.CompareTag("Pit") && canMove) {
            GameObject camera = GameObject.FindWithTag("MainCamera");
            GameManager.instance.lowerPlayerHealth(1);

            //sets player's position to the pit's respawn point
                if (collision.gameObject.transform.GetChild(0) != null)
                {
                    rb.position = collision.gameObject.transform.GetChild(0).position;
                }
                else
                {
                    rb.position = currentRoom;
                }
                sr.sprite = sprites[0];
                currentDirection = 0;
        }
        if (collision.CompareTag("Warp")) {
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
            currentDirection = 0;
        }
        if (collision.CompareTag("Item")) {
            GameObject item = collision.gameObject;
            GameManager.instance.receiveItem(item);
            //doesn't delete the item so that the gameobject can still be stored in gamemanager's items list
            //this is so the item's script can still be used
            item.tag = "Untagged";
            item.GetComponent<SpriteRenderer>().enabled = false;
            item.GetComponent<BoxCollider2D>().enabled = false;
            item.transform.SetParent(GameManager.instance.transform);
        }
        if (collision.CompareTag("BossRoom")) {
            musicPlayer.playBossMusic();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("BossRoom")) {
            musicPlayer.playSceneMusic();
        }
    }

    //this is so the player can't stand on top of pits after using the hammer
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Pit") && canMove) {
            GameObject camera = GameObject.FindWithTag("MainCamera");
            GameManager.instance.lowerPlayerHealth(1);

            //sets player's position to the pit's respawn point
            if (collision.gameObject.transform.GetChild(0) != null) {
                rb.position = collision.gameObject.transform.GetChild(0).position;
            }
            else {
                rb.position = currentRoom;
            }
            sr.sprite = sprites[0];
            currentDirection = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyManager enemy = collision.gameObject.GetComponent<EnemyManager>();
            if (canMove)
            {
                GameManager.instance.lowerPlayerHealth(1);
                // Knockback on enemy collision
                float playerKnockbackDistance = 1.0f;
                float enemyKnockbackDistance = 1.0f;
                float knockbackDuration = 0.12f;

                // direction from enemy to player
                Vector2 enemyPos = collision.gameObject.transform.position;
                Vector2 playerPos = rb.position;
                Vector2 dir = (playerPos - enemyPos);
                if (dir.sqrMagnitude < 0.0001f)
                {
                    // fallback if positions almost identical
                    dir = Vector2.up;
                }
                dir.Normalize();

                // push player back immediately
                rb.position = rb.position + dir * playerKnockbackDistance;

                // tell enemy to move the opposite direction (smoothly)
                if (enemy != null)
                {
                    enemy.ApplyKnockback(-dir, enemyKnockbackDistance, knockbackDuration);
                }
            }
            else
            {
                hitEnemy(enemy, collision.gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Chest")) {
            lastChestTouched = collision.gameObject;
        }

    }

    public void hitEnemy(EnemyManager enemy, GameObject enemyCollider) {
        enemy.lowerEnemyHealth(1);
        if (enemy.enemyHealth > 0) {
            float enemyKnockbackDistance = 2.5f;
            float knockbackDuration = 0.12f;

            // direction from enemy to player
            Vector2 enemyPos = enemyCollider.transform.position;
            Vector2 playerPos = rb.position;
            Vector2 dir = (playerPos - enemyPos);
            if (dir.sqrMagnitude < 0.0001f) {
                // fallback if positions almost identical
                dir = Vector2.up;
            }
            dir.Normalize();

            // tell enemy to move the opposite direction (smoothly)
            if (enemy != null) {
                enemy.ApplyKnockback(-dir, enemyKnockbackDistance, knockbackDuration);
            }
        }
    }

    public void setCanMove(bool b) {
        canMove = b;
    }
    public int getDirectionFacing() {
        return currentDirection;
    }
}
