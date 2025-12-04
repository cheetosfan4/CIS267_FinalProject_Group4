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
    private Vector2 entryPosition;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        currentRoom = Vector2.zero;
        entryPosition = Vector2.zero;
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
            Vector3 roomCoordinates = new Vector3();
            roomCoordinates.x = collision.gameObject.transform.position.x;
            roomCoordinates.y = collision.gameObject.transform.position.y;
            currentRoom = roomCoordinates;

            //sets an offset for the entry position,
            //so it is not on the border of two rooms
            entryPosition = rb.position;
            if (rb.position.x > roomCoordinates.x) {
                entryPosition.x -= 1;
            }
            else if (rb.position.x < roomCoordinates.x) {
                entryPosition.x += 1;
            }
            else if (rb.position.y > roomCoordinates.y) {
                entryPosition.y -= 1;
            }
            else if (rb.position.y < roomCoordinates.y) {
                entryPosition.y += 1;
            }
            else {
                entryPosition = roomCoordinates;
            }

            //camera defaults to -10 z
            roomCoordinates.z = -10f;

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

            //sets player's position to where they entered the room at
            rb.position = entryPosition;
            sr.sprite = sprites[0];

            /*
            Vector3 snapPosition = currentRoom;
            snapPosition.z = -10f;
            camera.GetComponent<CameraManager>().snapToRoom(snapPosition);*/
        }
        if (collision.CompareTag("Warp")) {
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
        }
    }
}
