using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }
    public GameObject playerPrefab;
    public GameObject menuButtons;
    public GameObject title;
    public GameObject health;
    public GameObject[] pips;
    public Sprite[] pipSprites;
    public GameObject startButton;
    private TextMeshProUGUI startButtonText;
    public GameObject exitButton;
    private TextMeshProUGUI exitButtonText;
    public GameObject inventory;
    public GameObject[] inventorySlots;
    public GameObject selector;
    private RectTransform selectorTransform;

    private bool gameLoaded;
    private bool gamePaused;
    private bool inInventory;
    private int currentSlot;
    private GameObject currentPlayer;
    private int playerHealth;
    private bool dead;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        startButtonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        exitButtonText = exitButton.GetComponentInChildren<TextMeshProUGUI>();
        selectorTransform = selector.GetComponent<RectTransform>();

        //for starting the game from a different scene in the editor
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            defaultInitialize();
        }
        else {
            title.SetActive(true);
            menuButtons.SetActive(true);
            inventory.SetActive(false);
            health.SetActive(false);
            gameLoaded = false;
            gamePaused = false;
            inInventory = false;
        }
    }

    private void defaultInitialize() {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<Rigidbody2D>().position = Vector2.zero;
        currentPlayer = player;
        playerHealth = 5;
        health.SetActive(true);
        updatePips();
        title.SetActive(false);
        menuButtons.SetActive(false);
        inventory.SetActive(false);
        dead = false;
        Time.timeScale = 1;
        gameLoaded = true;
        gamePaused = false;
        inInventory = false;
    }

    private void Update() {
        loadNewLevel();
        pauseGame();
        manageInventory();
    }

    public void startGame() {
        if (!gameLoaded || dead) {
            SceneManager.LoadScene("LevelOne");
            defaultInitialize();
        }
        else if (gamePaused) {
            menuButtons.SetActive(false);
            Time.timeScale = 1;
            gamePaused = false;
        }

    }

    public void exitGame() {
        if (gameLoaded) {
            SceneManager.LoadScene("MainMenu");
            menuButtons.SetActive(true);
            startButtonText.text = "Start";
            exitButtonText.text = "Exit";
            title.SetActive(true);
            inventory.SetActive(false);
            health.SetActive(false);
            gameLoaded = false;
        }
        else {
            Application.Quit();
        }
    }

    private void loadNewLevel() {
        if (gameLoaded && !dead && !inInventory) {
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
                defaultInitialize();
            }
        }
    }

    private void pauseGame() {
        //pause menu (resume, restart, exit)
        if (Input.GetKeyDown(KeyCode.T) && gameLoaded && !dead && !inInventory) {
            if (!gamePaused) {
                menuButtons.SetActive(true);
                startButtonText.text = "Resume";
                exitButtonText.text = "Main Menu";
                Time.timeScale = 0;
                gamePaused = true;
            }
            else {
                menuButtons.SetActive(false);
                Time.timeScale = 1;
                gamePaused = false;
            }
        }
        //inventory
        if (Input.GetKeyDown(KeyCode.R) && gameLoaded & !dead && !gamePaused) {
            if (!inInventory) {
                inventory.SetActive(true);
                Time.timeScale = 0;
                currentSlot = 0;
                selectorTransform.anchoredPosition = inventorySlots[currentSlot].GetComponent<RectTransform>().anchoredPosition;
                inInventory = true;
            }
            else {
                inventory.SetActive(false);
                Time.timeScale = 1;
                inInventory = false;
            }
        }
    }

    //idea for later:
    //when the player comes into contact with an item, it gets the collision.gameobject
    //each item will have an itemID in its script, which will be obtained from collision.gameobject.GetComponent<blabla>.getItemID();
    //this itemID is then sent from the player script to this script, which gives that ID to the next available inventory slot
    //(if same itemID already exists in table for stackable items it will just add to that stack)
    //add an array of sprites (or images?) to this script, and set the image component of each slot to the correct sprite/image using the itemID
    private void manageInventory() {
        if (inInventory) {
            //chooses desired slot
            //will later add the ability to move up and down
            if (Input.GetKeyDown(KeyCode.A)) {
                currentSlot--;
            }
            if (Input.GetKeyDown(KeyCode.D)) {
                currentSlot++;
            }

            //allows selector to wrap around the slot list
            if (currentSlot < 0) {
                currentSlot = 15;
            }
            else if (currentSlot >= inventorySlots.Length) {
                currentSlot = 0;
            }

            //sets selector position to the position of the desired slot
            selectorTransform.anchoredPosition = inventorySlots[currentSlot].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    public void lowerPlayerHealth(int amount) {
        playerHealth -= amount;
        Debug.Log("player health lowered by " + amount + ", current health: " + playerHealth);
        updatePips();
        if (playerHealth <= 0) {
            death();
        }
    }

    private void updatePips() {
        for (int i = pips.Length -1; i >= 0; i--) {
            if (playerHealth > i) {
                pips[i].GetComponent<Image>().sprite = pipSprites[0];
            }
            else {
                pips[i].GetComponent<Image>().sprite = pipSprites[1];
            }
        }
    }

    private void death() {
        dead = true;
        inventory.SetActive (false);
        menuButtons.SetActive(true);
        startButtonText.text = "Retry";
        exitButtonText.text = "Main Menu";
        Time.timeScale = 0;
    }
}
