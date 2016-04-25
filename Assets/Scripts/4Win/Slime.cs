using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour {

    void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.CompareTag("Bizarro")) {
            coll.gameObject.GetComponent<Bizarro>().slimed = true;
        }
    }

    void OnTriggerExit(Collider coll) {
        if (coll.gameObject.CompareTag("Bizarro")) {
            coll.gameObject.GetComponent<Bizarro>().slimed = false;
        }
    }

}
