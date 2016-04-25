using UnityEngine;
using System.Collections;

public class EvilAllBros : MonoBehaviour {

    private GameObject player;
    private GameObject bizarro;
    private GameObject laserEffect;
    private GameObject flameEffect;
    private GameObject beatingEffect;

    private float laserSpeed;
    private float flameSpeed;
    private float beatingSpeed;
    public GameObject[] broHolds = new GameObject[6];

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        bizarro = GameObject.FindGameObjectWithTag("Bizarro");
        flameEffect = (GameObject)Resources.Load("Effects/EvilSpecial/Burning");
        laserEffect = (GameObject)Resources.Load("Effects/EvilSpecial/BroLaser");
        beatingEffect = (GameObject)Resources.Load("Effects/EvilSpecial/MobBeating");
        laserSpeed = player.GetComponent<PlayerWin>().laserSpeed;
        flameSpeed = player.GetComponent<PlayerWin>().flameSpeed;
        beatingSpeed = player.GetComponent<PlayerWin>().beatingSpeed;
    }

    void Update() {
        transform.position = bizarro.transform.position;
    }

    public void Laser(Vector3 clickVector) {
        for (int i = 0; i < broHolds.Length; i++) {
            Vector3 shootVector = player.transform.position - broHolds[i].transform.position;
            if (shootVector.magnitude != 0)
                shootVector /= shootVector.magnitude;
            GameObject myLaser = (GameObject)Instantiate(laserEffect, broHolds[i].transform.position, Quaternion.identity);
            Destroy(myLaser, 10f);
            myLaser.GetComponent<Rigidbody>().velocity = shootVector * laserSpeed;
        }
        gameObject.SetActive(false);
    }

    public void Flames(Vector3 clickVector) {
        for (int i = 0; i < broHolds.Length; i++) {
            clickVector = Vector3.Normalize(clickVector);
            GameObject myFlame = (GameObject)Instantiate(flameEffect, broHolds[i].transform.position, Quaternion.identity);
            Destroy(myFlame, 10f);
            myFlame.GetComponent<Rigidbody>().velocity = clickVector * flameSpeed;
        }
        gameObject.SetActive(false);
    }

    public void Beating(Vector3 clickVector) {
        for (int i = 0; i < broHolds.Length; i++) {
            clickVector = Vector3.Normalize(clickVector);
            Vector3 shootVector = bizarro.transform.position - broHolds[i].transform.position;
            if (shootVector.magnitude != 0)
                shootVector /= shootVector.magnitude;
            GameObject myBeat = (GameObject)Instantiate(beatingEffect, broHolds[i].transform.position, Quaternion.identity);
            Destroy(myBeat, 10f);
            myBeat.GetComponent<Rigidbody>().velocity = shootVector * beatingSpeed;
        }
        gameObject.SetActive(false);

    }
}
