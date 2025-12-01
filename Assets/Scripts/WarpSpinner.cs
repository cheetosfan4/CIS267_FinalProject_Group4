using UnityEngine;

public class WarpSpinner : MonoBehaviour {
    public float spinSpeed;
    void Start() {
        
    }
    void Update() {
        this.gameObject.transform.Rotate(0, 0, spinSpeed *  Time.deltaTime);
    }
}
