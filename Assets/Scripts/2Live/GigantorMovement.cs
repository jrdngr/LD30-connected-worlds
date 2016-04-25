using UnityEngine;
using System.Collections;

public class GigantorMovement : MonoBehaviour {

    public float moveForce;
    public float maxSpeed;
    public float turboDistance;
    public float turboForce;
    public float turboMaxSpeed;
    public float bumpForce;
    public int bumpDamage;
    public float bumpTime;
    public float dazeTime;
    public float lavaBuoyancy;

    public Rect closeBox;

    private float moveX;
    private float moveY;
    private bool bumping;
    private bool dazed = false;
    private bool dead = false;
    private bool burning = false;
    private Vector3 bumpVector;
    private GameObject player;
    public GameObject fire;
    private GameManager gameManager;
    private PlayerMovement playerMovement;
    private Timer bumpTimer;
    private Timer dazeTimer;

    void OnDrawGizmosSelected() {
        closeBox.x = transform.position.x - closeBox.width/2;
        closeBox.y = transform.position.y - closeBox.height/2;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turboDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(closeBox.center, closeBox.size);
    }

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        fire.SetActive(false);
        playerMovement = player.GetComponent<PlayerMovement>();
        bumpTimer = gameObject.AddComponent<Timer>();
        bumpTimer.Trigger += BumpOff;
        dazeTimer = gameObject.AddComponent<Timer>();
        dazeTimer.Trigger += DazeOff;
    }

    void FixedUpdate() {
        if (gameManager.LevelStarted && !dazed && !dead) {
            closeBox.x = transform.position.x - closeBox.width / 2;
            closeBox.y = transform.position.y - closeBox.height / 2;
            float moveX = player.transform.position.x - transform.position.x;
            float moveY = player.transform.position.y - transform.position.y;
            float moveSpeed = new Vector2(moveX, moveY).magnitude;
            moveX /= moveSpeed;
            moveY /= moveSpeed;

            if (Vector3.Distance(transform.position, player.transform.position) > turboDistance) {
                if (player.transform.position.x > closeBox.xMax || player.transform.position.x < closeBox.xMin)
                    GetComponent<Rigidbody>().AddForce(new Vector2(moveX * turboForce, 0));
                if (player.transform.position.y > closeBox.yMax || player.transform.position.y < closeBox.yMin)
                    GetComponent<Rigidbody>().AddForce(new Vector2(0, moveY * turboForce));
                GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -turboMaxSpeed, turboMaxSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -turboMaxSpeed, turboMaxSpeed), 0);
            }
            else {
                if (player.transform.position.x > closeBox.xMax || player.transform.position.x < closeBox.xMin)
                    GetComponent<Rigidbody>().AddForce(new Vector2(moveX * moveForce, 0));
                if (player.transform.position.y > closeBox.yMax || player.transform.position.y < closeBox.yMin)
                    GetComponent<Rigidbody>().AddForce(new Vector2(0, moveY * moveForce));
                GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -maxSpeed, maxSpeed), 0);
            }
            if (closeBox.Contains(player.transform.position)) {
                GetComponent<Rigidbody>().AddForce(new Vector2(moveX, moveY) * moveForce);
            }
            if (bumping)
                Bump();
        }
        if (burning)
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -lavaBuoyancy));
    }

    void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("Buddy")) {
            if (!playerMovement.Afterglow && !playerMovement.Bashing)
                coll.gameObject.GetComponent<PlayerTraits>().Hit(bumpDamage);
        }
        if (coll.gameObject.CompareTag("Wall")) {
            Vector3 wallBump = coll.contacts[0].normal;
            wallBump.z = 0;
            wallBump.x = 0;
            GetComponent<Rigidbody>().AddForce(wallBump * 1000f);
        }
    }

    public void Burn() {
        dead = true;
        fire.SetActive(true);
        burning = true;
            GameObject.FindGameObjectWithTag("GM").GetComponent<LiveLevel>().EndLevel(true);
    }

    void Bump() {
        playerMovement.Bump(bumpVector);
        BumpMe(-bumpVector);
    }

    void BumpOff() {
        bumping = false;
        dazed = true;
        dazeTimer.Go(dazeTime);
    }

    public void BumpMe(Vector3 bumpForce) {
        bumpForce.z = 0;
        GetComponent<Rigidbody>().AddForce(bumpForce);
        dazed = true;
        dazeTimer.Go(dazeTime);
    }

    void DazeOff() {
        dazed = false;
    }

}
