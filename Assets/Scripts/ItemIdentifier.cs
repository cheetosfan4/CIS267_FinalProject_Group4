using UnityEngine;

public class ItemIdentifier : MonoBehaviour {
    //public int itemID;
    private bool equipped;

    private void Awake() {
        equipped = false;
    }

    public void setEquipped(bool b) {
        equipped = b;
    }

    public bool isEquipped() {
        return equipped;
    }
}
