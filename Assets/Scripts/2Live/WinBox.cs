using UnityEngine;
using System.Collections;

public class WinBox : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            GameObject.FindGameObjectWithTag("GM").GetComponent<LiveLevel>().EndLevel(false);
        }
    }

}
