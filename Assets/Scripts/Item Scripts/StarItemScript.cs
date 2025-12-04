using UnityEngine;

public class StarItemScript : MonoBehaviour {
    private ItemIdentifier itemID;
    private bool triggered;
    private Color color;
    private float hue;
    private GameObject player;
    private SpriteRenderer playerSR;

    //test script just to make sure that equipped items work
    private void Awake() {
        itemID = GetComponent<ItemIdentifier>();
        color = Color.white;
        //converts the color to HSV so the hue can be changed
        Color.RGBToHSV(color, out hue, out float s, out float v);
    }
    private void Update() {
        awaitUse();

        if (triggered) {
            //cycles through hues based on time
            //uses modulo 1 on the hue since hues only go from 0-1
            hue = (hue + Time.deltaTime) % 1f;

            playerSR.color = Color.HSVToRGB(hue, color.g, color.b);

            //in case the player unequips the item before untriggering it
            if (!itemID.isEquipped()) {
                triggered = false;
                playerSR.color = Color.white;
                Debug.Log("untriggered star effect");
            }
        }
    }

    private void awaitUse() {
        if (!GameManager.instance.isGamePaused() && !GameManager.instance.isDead() && !GameManager.instance.isInInventory()) {
            if (itemID.isEquipped()) {
                //player presses E to use the item
                if (Input.GetKeyDown(KeyCode.E)) {
                    GameObject player = GameManager.instance.getCurrentPlayer();

                    if (player != null) {
                        playerSR = player.GetComponentInChildren<SpriteRenderer>();

                        //toggle for the trigger boolean, so the effect can be turned on and off
                        if (triggered) {
                            triggered = false;
                            playerSR.color = Color.white;
                            Debug.Log("untriggered star effect");
                        }
                        else {
                            triggered = true;
                            Debug.Log("triggered star effect");
                        }
                    }
                }
            }
        }
    }
}
