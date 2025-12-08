using UnityEngine;

public class AppleScript : MonoBehaviour {
    private ItemIdentifier itemID;
    private bool triggered;

    private void Awake() {
        itemID = GetComponent<ItemIdentifier>();
    }
    private void Update() {
        awaitUse();

        if (triggered) {
            GameManager.instance.addPlayerHealth(2);
            Destroy(this.gameObject);
        }
    }

    private void awaitUse() {
        if (!GameManager.instance.isGamePaused() && !GameManager.instance.isDead() && !GameManager.instance.isInInventory()) {
            if (itemID.isEquipped()) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    triggered = true;
                }
            }
        }
    }
}
