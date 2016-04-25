using UnityEngine;
using System.Collections;

public class Bro : MonoBehaviour {

    public GameObject myCollider;
    public GameObject myTrigger;
    public GameObject myShieldAnchor;
    public GameObject myAnchor;
    public GameObject myShield;
    

    public bool shielding = false;
    
    private float moveX;
    private float moveY;
    private float moveForce;
    private float maxSpeed;

    private Vector3 myOffset;
    public Vector3 MyOffset {
        get { return myOffset; }
    }
    private GameObject player;
    private PlayerWin playerWin;
    private PlayerMovement playerMove;
    private GameManager gameManager;
    private GameObject bloodEffect;
    private GameObject bizarro;



    void Awake() {
        myShield.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        bizarro = GameObject.FindGameObjectWithTag("Bizarro");
        bloodEffect = (GameObject)Resources.Load("Effects/Special/MobBeating");
        playerWin = player.GetComponent<PlayerWin>();
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        playerMove = player.GetComponent<PlayerMovement>();
        myOffset = transform.position - player.transform.position;
        myOffset.z = 0;
        myCollider.SetActive(false);
        moveForce = playerMove.moveForce;
        maxSpeed = playerMove.maxSpeed;
    }
   
    void FixedUpdate() {
      /*moveX = playerMove.MoveX;
        moveY = playerMove.MoveY;
        rigidbody.AddForce(new Vector2(moveX * moveForce, moveY * moveForce));
        rigidbody.velocity = new Vector3(Mathf.Clamp(rigidbody.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rigidbody.velocity.y, -maxSpeed, maxSpeed), 0);*/
        if (shielding) {
            myShield.SetActive(true);
            transform.position = myShieldAnchor.transform.position;
        }
        else {
            myShield.SetActive(false);
            transform.position = myAnchor.transform.position;
        }
    }

    public void Kill() {
        GameObject myBlood = (GameObject)Instantiate(bloodEffect, transform.position, Quaternion.identity);
        Destroy(myBlood, 5f);
        gameObject.SetActive(false);
    }
}
