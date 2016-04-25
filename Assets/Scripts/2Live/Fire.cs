using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

    private GameObject gigantor;

    void OnEnable() {
        gigantor = transform.parent.gameObject;
        transform.parent = null;
    }

    void Update() {
        if (transform.parent == null) {
            Vector3 newPos = gigantor.transform.position;
            newPos.x += 0.5f;
            newPos.z = -9f;
            transform.position = newPos;
        }
    }
}
