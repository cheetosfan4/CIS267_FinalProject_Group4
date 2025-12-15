using UnityEngine;

public class CannonScript : MonoBehaviour {
    public GameObject cannonBallPrefab;
    public AudioClip cannonShot;
    private ItemIdentifier itemID;
    private GameObject player;
    public float cannonShootLength;
    public Sprite[] cannonShootSprites;
    private float cannonTimer;
    private GameObject cannonBall;
    private bool shot;
    bool reset;

    void Awake() {
        itemID = GetComponent<ItemIdentifier>();
        reset = false;
        cannonTimer = 0;
        shot = false;
    }
    void Update() {
        awaitUse();

        if (player != null && itemID.isEquipped()) {
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

            if (cannonBall == null && cannonTimer <= 0 && shot) {
                shot = false;
                playerScript.setCanMove(false);
                cannonTimer = cannonShootLength;
                cannonBall = Instantiate(cannonBallPrefab);
                cannonBall.transform.position = playerRB.transform.position;
                cannonBall.transform.position += new Vector3(horizontal, vertical, 0);
                cannonBall.GetComponent<CannonBallManager>().setDirection(new Vector2(horizontal, vertical));
                cannonBall.GetComponent<CannonBallManager>().setReady();
                //playerSR.sprite = cannonShootSprites[playerDirection];
            }
            reset = false;
            cannonTimer -= Time.deltaTime;
        }
        if ((cannonTimer <= 0 || player == null || !itemID.isEquipped()) && !reset) {
            reset = true;
            //Debug.Log("conditions not met");
            cannonTimer = 0;
            player = GameManager.instance.getCurrentPlayer();
            player.GetComponent<PlayerManager>().setCanMove(true);
            Sprite[] sprites = player.GetComponent<PlayerManager>().sprites;
            //resets player sprite in case it gets stuck as one of the cannon ones
            player.GetComponentInChildren<SpriteRenderer>().sprite = sprites[player.GetComponent<PlayerManager>().getDirectionFacing()];
        }
    }

    private void awaitUse() {
        if (!GameManager.instance.isGamePaused() && !GameManager.instance.isDead() && !GameManager.instance.isInInventory()) {
            if (itemID.isEquipped()) {
                //player presses E to use the item
                //only lets player use the item again once the timer has reached 0
                if (Input.GetKeyDown(KeyCode.E) && cannonTimer <= 0) {
                    //Debug.Log("registered cannon press");
                    player = GameManager.instance.getCurrentPlayer();
                    shot = true;
                    AudioSource.PlayClipAtPoint(cannonShot, player.transform.position);
                    //Debug.Log("cannon timer set to " + cannonTimer);
                }
            }
        }
    }
}
