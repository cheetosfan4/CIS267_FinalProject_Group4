using UnityEngine;

public class ChestManager : MonoBehaviour {
    public GameObject itemToSpawn;

    public void open() {
        GameObject item = Instantiate(itemToSpawn);
        item.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }
}
