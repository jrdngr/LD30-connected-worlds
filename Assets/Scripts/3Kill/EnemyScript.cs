using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

    public float moveForce;
    public float maxSpeed;
    public float detectionRadiusModifier;
    public float allyDetectionRadius;
    public float bumpForce;
    public bool targeted = false;

    private float moveX;
    private float moveY;
    private float playerDetectionRadius = 0;
    private bool seesPlayer = false;
    private bool hasTarget = false;
    private bool noTargets = false;
    private GameObject target;
    private GameObject player;
    private GameManager gameManager;
    private PlayerTraits playerTraits;
    private Death myDeathScript;

    
    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTraits = player.GetComponent<PlayerTraits>();
        myDeathScript = GetComponent<Death>();
        GetComponent<Renderer>().sortingLayerID = 0;
    }

    void Update() {
        playerDetectionRadius = playerTraits.EnemiesKilled / detectionRadiusModifier;
        if (!hasTarget && !noTargets)
            FindTarget();
    }

    void FixedUpdate(){
        if (gameManager.LevelStarted) {
            if (!myDeathScript.IsBurning) {
                if (playerDetectionRadius > 0)
                    SearchForPlayer();
                if (seesPlayer) {
                    float playerVectorMagnitude;
                    playerVectorMagnitude = (player.transform.position - transform.position).magnitude;
                    moveX = (transform.position.x - player.transform.position.x) / playerVectorMagnitude;
                    moveY = (transform.position.y - player.transform.position.y) / playerVectorMagnitude;
                    GetComponent<Rigidbody>().AddForce(new Vector2(moveX * moveForce, moveY * moveForce));
                    GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -maxSpeed, maxSpeed), 0);
                }
                else {
                    if (target == null) {
                        hasTarget = false;
                    }
                    else {
                        float targetVectorMagnitude;
                        targetVectorMagnitude = (target.transform.position - transform.position).magnitude;
                        moveX = (target.transform.position.x - transform.position.x) / targetVectorMagnitude;
                        moveY = (target.transform.position.y - transform.position.y) / targetVectorMagnitude;
                        GetComponent<Rigidbody>().AddForce(new Vector2(moveX * moveForce, moveY * moveForce));
                        GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -maxSpeed, maxSpeed), 0);
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("Ally") || coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("Enemy")) {
            Vector3 bumpVector = coll.contacts[0].normal;
            bumpVector.z = 0;
            GetComponent<Rigidbody>().AddForce(bumpVector * bumpForce);
        }
    }

    void FindTarget() {
            Collider[] colliders = Physics.OverlapSphere(transform.position, allyDetectionRadius);
        foreach (Collider c in colliders) {
            if (c.gameObject.CompareTag("Ally")) {
                if (!c.gameObject.GetComponent<AllyScript>().targeted) {
                    if (target != null) {
                        if ((c.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)
                            target = c.gameObject;
                    }
                    else
                        target = c.gameObject;
                }
            }
        }
        if (target != null) {
            hasTarget = true;
            target.GetComponent<AllyScript>().targeted = true;
        }
        else
            noTargets = true;
    }

    void SearchForPlayer() {
        seesPlayer = (Vector3.Distance(player.transform.position, transform.position) < playerDetectionRadius) ? true : false;
    }
}
