using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager instance { get; private set; }
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public float moveSpeed;
    public Sprite[] sprites;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float inputHorizontal;
    private float inputVertical;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update() {
        movePlayer();
        loadNewLevel();
    }

    private void movePlayer() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        rb.linearVelocity = new Vector2(moveSpeed * inputHorizontal, moveSpeed * inputVertical);
        flipPlayer();
    }

    private void flipPlayer() {
        int direction = 99;
        if (inputHorizontal > 0) {
            direction = 3;
        }
        else if (inputHorizontal < 0){
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

    private void loadNewLevel() {
        //this moves the player between all of the different levels
        //just for testing
        bool sceneLoaded = false;
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            SceneManager.LoadScene("Testing");
            sceneLoaded = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SceneManager.LoadScene("LevelOne");
            sceneLoaded = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SceneManager.LoadScene("LevelTwo");
            sceneLoaded = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SceneManager.LoadScene("LevelThree");
            sceneLoaded = true;
        }

        if (sceneLoaded) {
            rb.position = new Vector2(0, 0);
            sr.sprite = sprites[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("RoomPresence")) {
            //each room has a "room_presence" object that has a trigger collider
            //when the player enters a new room, the location of the room is sent to the camera
            GameObject camera = GameObject.FindWithTag("MainCamera");
            Vector3 roomCoordinates = new Vector2();
            roomCoordinates.x = collision.gameObject.transform.position.x;
            roomCoordinates.y = collision.gameObject.transform.position.y;
            //camera defaults to -10 z
            roomCoordinates.z = -10f;

            if (camera != null) {
                camera.GetComponent<CameraManager>().moveToRoom(roomCoordinates);
            }
            else {
                Debug.Log("camera not found");
            }
        }
    }
}
