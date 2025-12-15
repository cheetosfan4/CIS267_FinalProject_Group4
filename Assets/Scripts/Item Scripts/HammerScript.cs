using System.Drawing;
using System.Dynamic;
using UnityEngine;
//using static UnityEditor.Progress;

public class HammerScript : MonoBehaviour {
    public AudioClip strike;
    private ItemIdentifier itemID;
    private GameObject player;
    public float hammerSpeed;
    public float hammerTimerLength;
    //public Sprite[] hammerWieldSprites;
    private float hammerTimer;
    bool reset;

    void Awake() {
        itemID = GetComponent<ItemIdentifier>();
        reset = false;
    }
    void Update() {
        awaitUse();

        if (hammerTimer > 0 && player != null && itemID.isEquipped()) {
            //Debug.Log("conditions met");
            PlayerManager playerScript = player.GetComponent<PlayerManager>();
            Rigidbody2D playerRB = playerScript.GetComponent<Rigidbody2D>();
            SpriteRenderer playerSR = playerScript.GetComponentInChildren<SpriteRenderer>();
            int playerDirection = playerScript.getDirectionFacing();
            float horizontal = 0;
            float vertical = 0;

            switch (playerDirection) {
                //front
                case 0:
                    vertical = -1;
                    break;
                //back
                case 1:
                    vertical = 1;
                    break;
                //left
                case 2:
                    horizontal = -1;
                    break;
                //right
                case 3:
                    horizontal = 1;
                    break;
            }

            playerScript.setCanMove(false);
            playerRB.linearVelocity = new Vector2(hammerSpeed * horizontal, hammerSpeed * vertical);
            //playerSR.sprite = hammerWieldSprites[playerDirection];
            reset = false;
            hammerTimer -= Time.deltaTime;
        }
        else if ((hammerTimer <= 0 || player == null || !itemID.isEquipped()) && !reset) {
            reset = true;
            //Debug.Log("conditions not met");
            hammerTimer = 0;
            player = GameManager.instance.getCurrentPlayer();
            player.GetComponent<PlayerManager>().setCanMove(true);
            //Sprite[] sprites = player.GetComponent<PlayerManager>().sprites;
            //resets player sprite in case it gets stuck as one of the hammer ones
            //player.GetComponentInChildren<SpriteRenderer>().sprite = sprites[player.GetComponent<PlayerManager>().getDirectionFacing()];
        }
    }

    private void awaitUse() {
        if (!GameManager.instance.isGamePaused() && !GameManager.instance.isDead() && !GameManager.instance.isInInventory()) {
            if (itemID.isEquipped()) {
                //player presses E to use the item
                //only lets player use the item again once the timer has reached 0
                if (Input.GetKeyDown(KeyCode.E) && hammerTimer <= 0) {
                    //Debug.Log("registered hammer press");
                    player = GameManager.instance.getCurrentPlayer();
                    hammerTimer = hammerTimerLength;
                    AudioSource.PlayClipAtPoint(strike, player.transform.position, 1);
                    //Debug.Log("hammer timer set to " + hammerTimer);
                }
            }
        }
    }
}
