using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public float moveSpeed;
    public Sprite[] sprites;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float inputHorizontal;
    private float inputVertical;
    private Vector2 currentRoom;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        currentRoom = Vector2.zero;
    }

    void Update() {
        movePlayer();
    }

    private void movePlayer() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        rb.linearVelocity = new Vector2(moveSpeed * inputHorizontal, moveSpeed * inputVertical);
        flipPlayer();
    }

    private void flipPlayer() {
        if (Time.timeScale != 0) {
            int direction = 99;
            if (inputHorizontal > 0) {
                direction = 3;
            }
            else if (inputHorizontal < 0) {
                direction = 2;
            }
            //separate if statement for vertical flipping,
            //so the front and back sprites take priority over the side sprites
            //may change how this works in the future
            if (inputVertical > 0) {
                direction = 1;
            }
            else if (inputVertical < 0) {
                direction = 0;
            }

            //only changes direction if any of the axes are greater than zero,
            //so the player doesnt flip when not moving
            if (direction != 99) {
                sr.sprite = sprites[direction];
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
        }
        if (collision.CompareTag("GateToThree")) {
            SceneManager.LoadScene("LevelThree");
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
        }
        if (collision.CompareTag("Pit")) {
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
        }
        if (collision.CompareTag("Warp")) {
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
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
    }
}
