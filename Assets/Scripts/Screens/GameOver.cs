using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {


	void Update () {
        if (Input.GetButton("Fire1"))
            Application.LoadLevel("1Eat");
	}
}
