using UnityEngine;

public class ArtifactTrioScript : MonoBehaviour {

    public GameObject burstPrefab;
    public AudioClip burstShot;
    private ItemIdentifier itemID;
    private GameObject player;
    public float burstShootLength;
    public Sprite[] burstShootSprites;
    private float burstTimer;
    private GameObject burst;
    private bool shot;
    bool reset;

    void Awake() {
        itemID = GetComponent<ItemIdentifier>();
        reset = false;
        burstTimer = 0;
        shot = false;
    }
    void Update() {
        awaitUse();

        if (player != null && itemID.isEquipped()) {
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

            if (burst == null && burstTimer <= 0 && shot) {
                shot = false;
                playerScript.setCanMove(false);
                burstTimer = burstShootLength;
                burst = Instantiate(burstPrefab);
                burst.transform.position = playerRB.transform.position;
                burst.transform.position += new Vector3(horizontal, vertical, 0);
                burst.GetComponent<ArtifactBurstScript>().setReady();
                //playerSR.sprite = burstShootSprites[playerDirection];
            }
            reset = false;
            burstTimer -= Time.deltaTime;
        }
        if ((burstTimer <= 0 || player == null || !itemID.isEquipped()) && !reset) {
            reset = true;
            burstTimer = 0;
            player = GameManager.instance.getCurrentPlayer();
            player.GetComponent<PlayerManager>().setCanMove(true);
            Sprite[] sprites = player.GetComponent<PlayerManager>().sprites;
            //resets player sprite in case it gets stuck as one of the burst ones
            player.GetComponentInChildren<SpriteRenderer>().sprite = sprites[player.GetComponent<PlayerManager>().getDirectionFacing()];
        }
    }

    private void awaitUse() {
        if (!GameManager.instance.isGamePaused() && !GameManager.instance.isDead() && !GameManager.instance.isInInventory()) {
            if (itemID.isEquipped()) {
                if (Input.GetKeyDown(KeyCode.E) && burstTimer <= 0) {
                    player = GameManager.instance.getCurrentPlayer();
                    shot = true;
                    AudioSource.PlayClipAtPoint(burstShot, player.transform.position);
                }
            }
        }
    }
}
