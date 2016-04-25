using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {


    void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Gigantor")) {
            other.GetComponent<GigantorMovement>().Burn();
        }
        if (other.gameObject.CompareTag("Player")) {
            other.GetComponent<PlayerTraits>().Kill();
        }
    }

}
