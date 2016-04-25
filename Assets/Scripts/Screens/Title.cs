using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {

    void Update() {
        if (Input.GetButton("Fire1"))
            Application.LoadLevel("1Eat");
    }

}
