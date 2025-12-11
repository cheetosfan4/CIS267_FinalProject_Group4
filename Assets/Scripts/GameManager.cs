using System.Collections.Generic;
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
    private List<GameObject> items;
    public GameObject equippedItemSlot;
    private GameObject equippedItem;
    public MusicManager musicPlayer;
    public GameObject artifact1;
    public GameObject artifact2;
    public GameObject artifact3;
    public Sprite[] artifactSprites;
    public GameObject artifactTrio;

    private bool gameLoaded;
    private bool gamePaused;
    private bool inInventory;
    private int currentSlot;
    private GameObject currentPlayer;
    private int playerHealth;
    private bool dead;
    private float invulnerabilityTimer;
    private int artifactsCollected;

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

        items = new List<GameObject>();

        //for starting the game from a different scene in the editor
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            //defaultInitialize();
        }
        //for starting the game from the main menu
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

    //runs defaultInitialize() once each gameplay scene is loaded
    //currentPlayer wasn't getting properly set since it was running before the scene was fully loaded
    private void OnEnable() {
        SceneManager.sceneLoaded += onSceneLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }
    private void onSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != "MainMenu") {
            defaultInitialize();
        }
        musicPlayer.playSceneMusic();
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
        invulnerabilityTimer = 0;
    }

    //destroys all items the player has collected
    //is only called when quitting to main menu, and starting a new game
    private void clearItems() {
        if (equippedItem != null) {
            Destroy(equippedItem);
            equippedItem = null;
        }
        for (int i = 0; i < items.Count; i++) {
            Destroy(items[i]);
        }
        items.Clear();
    }

    private void Update() {
        loadNewLevel();
        pauseGame();
        manageInventory();

        //this timer is so the player can't take any more damage for a brief period after getting hit
        if (invulnerabilityTimer > 0) {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    public void startGame() {
        //starting the game from the main menu
        if (!gameLoaded || dead) {
            SceneManager.LoadScene("LevelOne");
            //defaultInitialize();
            clearItems();
            artifactsCollected = 0;
        }
        //resuming the game from pause screen
        else if (gamePaused) {
            menuButtons.SetActive(false);
            Time.timeScale = 1;
            gamePaused = false;
        }
        //need to add a restart button for the pause menu
    }

    public void exitGame() {
        //returns to main menu from pause screen
        if (gameLoaded) {
            SceneManager.LoadScene("MainMenu");
            menuButtons.SetActive(true);
            startButtonText.text = "Start";
            exitButtonText.text = "Exit";
            title.SetActive(true);
            inventory.SetActive(false);
            health.SetActive(false);
            gameLoaded = false;
            clearItems();
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
                //defaultInitialize();
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
                loadItems();
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

    public void artifactCollected() {
        artifactsCollected++;
        if (artifactsCollected == 3) {
            GameObject artifactItem = Instantiate(artifactTrio);
            artifactItem.transform.position = currentPlayer.transform.position;
        }
    }

    private void updateArtifacts() {
        artifact1.GetComponent<Image>().sprite = artifactSprites[0];
        artifact2.GetComponent<Image>().sprite = artifactSprites[0];
        artifact3.GetComponent<Image>().sprite = artifactSprites[0];
        switch (artifactsCollected) {
            case 0: {
                    break;
                }
            case 1: {
                    artifact1.GetComponent<Image>().sprite = artifactSprites[1];
                    break;
                }
            case 2: {
                    artifact1.GetComponent<Image>().sprite = artifactSprites[1];
                    artifact2.GetComponent<Image>().sprite = artifactSprites[1];
                    break;
                }
            case 3: {
                    artifact1.GetComponent<Image>().sprite = artifactSprites[1];
                    artifact2.GetComponent<Image>().sprite = artifactSprites[1];
                    artifact3.GetComponent<Image>().sprite = artifactSprites[1];
                    break;
                }
        }
    }

    private void loadItems() {
        updateArtifacts();
        //disables all inventory slot images before loading them, in case the player lost any items
        for (int i = 0; i < inventorySlots.Length; i++) {
            inventorySlots[i].GetComponent<Image>().enabled = false;
        }
        equippedItemSlot.GetComponent<Image>().enabled = false;

        for (int i = 0; i < items.Count; i++) {
            Image slotImage = inventorySlots[i].GetComponent<Image>();
            slotImage.enabled = true;
            slotImage.sprite = items[i].GetComponent<SpriteRenderer>().sprite;
        }
        if (equippedItem != null) {
            Image slotImage = equippedItemSlot.GetComponent<Image>();
            slotImage.enabled = true;
            slotImage.sprite = equippedItem.GetComponent<SpriteRenderer>().sprite;
        }
    }

    //!!!need to implement stacking items
    //and possibly a failsafe for collecting more unique items than there are slots
    private void manageInventory() {
        if (inInventory) {
            bool equip = false;
            bool swap = false;
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

            if (Input.GetKeyDown(KeyCode.E)) {
                //if the image component is enabled, that means there is an item in that slot
                if (inventorySlots[currentSlot].GetComponent<Image>().enabled == true) {
                    equip = true;
                }
                else if (inventorySlots[currentSlot].GetComponent<Image>().enabled == false && equippedItem != null) {
                    swap = true;
                }
            }

            //sets selector position to the position of the desired slot
            selectorTransform.anchoredPosition = inventorySlots[currentSlot].GetComponent<RectTransform>().anchoredPosition;
            
            //equips selected item if player pressed E
            if (equip && equippedItem == null) {
                equippedItem = items[currentSlot];
                items.Remove(equippedItem);
                equippedItem.GetComponent<ItemIdentifier>().setEquipped(true);
                loadItems();
            }
            else if (equip && equippedItem != null) {
                GameObject oldItem = equippedItem;
                items.Add(oldItem);
                equippedItem = items[currentSlot];
                items.Remove(equippedItem);
                equippedItem.GetComponent<ItemIdentifier>().setEquipped(true);
                oldItem.GetComponent<ItemIdentifier>().setEquipped(false);
                loadItems();
            }
            else if (swap) {
                items.Add(equippedItem);
                equippedItem.GetComponent<ItemIdentifier>().setEquipped(false);
                equippedItem = null;
                loadItems();
            }
        }
    }

    public void lowerPlayerHealth(int amount) {
        if (invulnerabilityTimer <= 0) {
            playerHealth -= amount;
            Debug.Log("player health lowered by " + amount + ", current health: " + playerHealth);
            updatePips();

            if (playerHealth <= 0) {
                death();
            }
            else {
                invulnerabilityTimer = 0.25f;
            }
        }
    }

    public void addPlayerHealth(int amount) {
        playerHealth += amount;
        if (playerHealth > 5) {
            playerHealth = 5;
        }
        Debug.Log("player health increased by " + amount + ", current health: " + playerHealth);
        updatePips();
    }

    private void updatePips() {
        //starts at end of pips array
        for (int i = pips.Length -1; i >= 0; i--) {
            //maximum of i is 4, maximum for player health is 5
            if (playerHealth > i) {
                pips[i].GetComponent<Image>().sprite = pipSprites[0];
            }
            else {
                pips[i].GetComponent<Image>().sprite = pipSprites[1];
            }
        }
    }

    public void receiveItem(GameObject item) {
        items.Add(item);
    }

    private void death() {
        dead = true;
        inventory.SetActive (false);
        menuButtons.SetActive(true);
        startButtonText.text = "Retry";
        exitButtonText.text = "Main Menu";
        Time.timeScale = 0;
    }
    public bool isDead() {
        return dead;
    }
    public bool isGamePaused() {
        return gamePaused;
    }
    public bool isInInventory() {
        return inInventory;
    }

    public GameObject getCurrentPlayer() {
        return currentPlayer;
    }
}
