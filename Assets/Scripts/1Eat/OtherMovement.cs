using UnityEngine;
using System.Collections;

public class OtherMovement : MonoBehaviour {

    public float foodDetectionRadius;
    public float moveForce;
    public float maxSpeed;

    private float moveX;
    private float moveY;
    private int playerDetectionRadius;
    private bool hasTarget = false;
    private bool noTargets = false;
    private bool seesPlayer = false;
    private OtherFood otherFood;
    private EatLevel gameManager;
    private GameManager gameManagerStupid;
    private GameObject target;
    private GameObject player;


    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, foodDetectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }

    void Awake() {
        otherFood = GetComponent<OtherFood>();
        gameManagerStupid = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        gameManager = gameManagerStupid.GetComponent<EatLevel>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (!hasTarget && !noTargets && !otherFood.FoodDelay)
            FindTarget();
    }

    void FixedUpdate() {
        if (gameManagerStupid.LevelStarted) {
            playerDetectionRadius = gameManager.EnemyFear;
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

    void SearchForPlayer() {
        seesPlayer = (Vector3.Distance(player.transform.position, transform.position) < playerDetectionRadius) ? true : false;
    }    

    void FindTarget() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, foodDetectionRadius);
        foreach (Collider c in colliders) {
            if (c.gameObject.CompareTag("Food")) {
                if (target != null) {
                    if ((c.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)
                        target = c.gameObject;
                }
                else
                    target = c.gameObject;
            }
        }
        if (target != null)
            hasTarget = true;
        else
            noTargets = true;
    }

}
