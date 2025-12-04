using UnityEngine;

public class CameraManager : MonoBehaviour {
    public float cameraSpeed;
    private SpriteRenderer sr;
    private Vector3 roomTarget;
    private void Awake() {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.enabled = true;
        roomTarget = transform.position;
    }

    private void Update() {
        if (transform.position != roomTarget) {
            transform.position = Vector3.MoveTowards(transform.position, roomTarget, cameraSpeed * Time.deltaTime);
        }
    }

    public void moveToRoom(Vector3 coordinates) {
        roomTarget = coordinates;
        roomTarget.z = -10f;
    }

    public void snapToRoom(Vector3 coordinates) {
        roomTarget = coordinates;
        roomTarget.z = -10f;
        transform.position = roomTarget;
    }
}
