using UnityEngine;
using System.Collections;

public class AllyScript : MonoBehaviour {

    public float moveForce;
    public float maxSpeed;
    public float enemyDetectionRadius;
    public float bumpForce;
    public bool targeted = false;

    private float moveX;
    private float moveY;
    private bool hasTarget = false;
    private bool noTargets = false;
    private GameObject target;
    private GameManager gameManager;
    private Death myDeathScript;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        myDeathScript = GetComponent<Death>();
        GetComponent<Renderer>().sortingLayerName = "Objects";
    }

    void Update() {
        if (!hasTarget && !noTargets)
            FindTarget();
    }

    void FixedUpdate() {
        if (gameManager.LevelStarted) {
            if (!myDeathScript.IsBurning) {
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

    void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("Ally") || coll.gameObject.CompareTag("Player") || coll.gameObject.CompareTag("Enemy")) {
            Vector3 bumpVector = coll.contacts[0].normal;
            bumpVector.z = 0;
            GetComponent<Rigidbody>().AddForce(bumpVector * bumpForce);
        }
    }

    void FindTarget() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);
        foreach (Collider c in colliders) {
            if (c.gameObject.CompareTag("Enemy")) {
                if (!c.gameObject.GetComponent<EnemyScript>().targeted) {
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
            target.GetComponent<EnemyScript>().targeted = true;
        }
        else
            noTargets = true;
    }

}
