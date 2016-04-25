using UnityEngine;
using System.Collections;

public class Bizarro : MonoBehaviour {

    public float moveForce;
    public float maxSpeed;
    public float dazeTime;
    public float bumpTime;
    public GameObject deathEffect;
    public TextMesh bizarroHealthDisplay;
    
    public bool slimed = false;

    private float moveX;
    private float moveY;
    private float currentMoveSpeed;

    private int health;
    private bool dazed = false;
    private bool bumping = false;
    private Vector3 bumpForce;
    private bool carnivore = false;
    private bool killer = false;
    private bool hasAllies = false;
    private GameObject teleportEffect;
    private GameObject bloodEffect;
    private GameObject player;
    private GameManager gameManager;
    private Timer dazeTimer;
    private Timer bumpTimer;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        teleportEffect = (GameObject)Resources.Load("Effects/KillEffects/TeleportEnd");
        bloodEffect = (GameObject)Resources.Load("Effects/Special/MobSplat");
        health = PlayerPrefs.GetInt("Health") * 3;
        dazeTimer = gameObject.AddComponent<Timer>();
        dazeTimer.Trigger += DazeOff;
        bumpTimer = gameObject.AddComponent<Timer>();
        bumpTimer.Trigger += BumpOff;
        GetComponent<Renderer>().enabled = false;
        currentMoveSpeed = maxSpeed;

        //DEBUG
        //health = 500;
        //DEBUG
               
    }

    void Update() {
        bizarroHealthDisplay.text = "Bizarro Health: " + health.ToString();
        if (slimed) {
            currentMoveSpeed = 1;
        }
        else
            currentMoveSpeed = maxSpeed;
        if (health <= 0)
            KillMe();
    }

    void FixedUpdate() {
        if (bumping) {
            Bumping();
        }
        if (gameManager.LevelStarted && !dazed) {
            float moveX = player.transform.position.x - transform.position.x;
            float moveY = player.transform.position.y - transform.position.y;
            float moveSpeed = new Vector2(moveX, moveY).magnitude;
            moveX /= moveSpeed;
            moveY /= moveSpeed;
            GetComponent<Rigidbody>().AddForce(new Vector2(moveX * moveForce, moveY * moveForce));
            GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -currentMoveSpeed, currentMoveSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -currentMoveSpeed, currentMoveSpeed), 0);
        }
    }

    void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("Shield")) {
            coll.gameObject.transform.parent.gameObject.GetComponent<Bro>().Kill();
            Vector3 thisBumpVector = coll.contacts[0].normal;
            if (thisBumpVector.magnitude != 0)
                thisBumpVector /= thisBumpVector.magnitude;
            Hit(10, thisBumpVector* 1500);
        }
        if (coll.gameObject.CompareTag("Laser") || coll.gameObject.CompareTag("Fire")) {
            health -= 10;
            coll.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        }
        if (coll.gameObject.CompareTag("Beat")) {
            health -= 30;
            GameObject blood = (GameObject)Instantiate(bloodEffect, coll.transform.position, Quaternion.identity);
            Destroy(blood, 5f);
            Destroy(coll.gameObject);
        }
    }

    void DazeOff() {
        dazed = false;
    }

    void BumpOff() {
        bumping = false;
    }

    void Bumping() {
        GetComponent<Rigidbody>().AddForce(bumpForce);
    }

    void KillMe() {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        gameManager.GetComponent<WinLevel>().NextLevel();
        Destroy(this.gameObject);
    }

    public void Hit(int damage, Vector3 bForce) {
        dazed = true;
        dazeTimer.Go(dazeTime);
        bumping = true;
        bumpTimer.Go(bumpTime);
        health -= damage;
        bumpForce = bForce;
        
    }

    public void TeleportIn() {
        GameObject tpEffect = (GameObject)Instantiate(teleportEffect, transform.position, Quaternion.Euler(0, 180, 180));
        Destroy(tpEffect, 5f);
        GetComponent<Renderer>().enabled = true;
    }

    public void SetTraits(bool carn, bool killah, bool coolguy){
        carnivore = !carn;
        killer = !killah;
        hasAllies = !coolguy;
    }

    public void DazeMe(float dazeMeTime) {
        dazed = true;
        dazeTimer.Go(dazeMeTime);
    }

}
