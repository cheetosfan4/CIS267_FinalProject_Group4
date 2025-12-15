using UnityEngine;

public class ChestManager : MonoBehaviour {
    public GameObject itemToSpawn;
    public AudioClip chestOpen;

    public void open() {
        GameObject item = Instantiate(itemToSpawn);
        item.transform.position = this.transform.position;
        AudioSource.PlayClipAtPoint(chestOpen, transform.position);
        Destroy(this.gameObject);
    }
}
