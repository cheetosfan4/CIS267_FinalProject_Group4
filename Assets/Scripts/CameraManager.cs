using UnityEngine;

public class CameraManager : MonoBehaviour {
    private SpriteRenderer sr;
    private void Awake() {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.enabled = true;
    }
    public void moveToRoom(Vector3 coordinates) {
        this.transform.position = coordinates;
    }
}
