using UnityEngine;

public class CameraManager : MonoBehaviour {
    public void moveToRoom(Vector3 coordinates) {
        this.transform.position = coordinates;
    }
}
