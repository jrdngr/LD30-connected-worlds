using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {

    public Rect moveBox;
    public Rect centerBox;
    
    public float cameraCenterDelay;
    public float cameraRecenterForce;
    
    private float moveForce;
    private float maxSpeed;
    private float moveX;
    private float moveY;
    private float startHorizontalMove = 0f;
    private float startVerticalMove = 0f;
    private bool centerX = false;
    private bool centerY = false;


    private int playerHealth;
    private TextMesh healthDisplay;
    private GameObject player;
    private PlayerTraits playerTraits;
    private PlayerMovement playerMovementScript;
    private Camera thisCamera;


    void OnDrawGizmosSelected() {
        moveBox.x = transform.position.x - moveBox.width/2;
        moveBox.y = transform.position.y - moveBox.height/2;
        centerBox.x = transform.position.x - centerBox.width / 2;
        centerBox.y = transform.position.y - centerBox.height / 2;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(moveBox.center, moveBox.size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerBox.center, centerBox.size);
    }

    void Awake() {
        thisCamera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTraits = player.GetComponent<PlayerTraits>();
        playerMovementScript = player.GetComponent<PlayerMovement>();
        healthDisplay = transform.Find("HealthDisplay").GetComponent<TextMesh>();
        thisCamera.orthographicSize = 10;

    }

    void Start() {
        moveForce = playerMovementScript.moveForce;
        maxSpeed = playerMovementScript.maxSpeed;
    }

    void Update() {
        playerHealth = playerTraits.Health;
        healthDisplay.text = "Health: " + playerHealth.ToString();
    }

    void FixedUpdate() {
        moveBox.x = transform.position.x - moveBox.width / 2;
        moveBox.y = transform.position.y - moveBox.height / 2;
        centerBox.x = transform.position.x - centerBox.width / 2;
        centerBox.y = transform.position.y - centerBox.height / 2;
        moveX = playerMovementScript.MoveX;
        moveY = playerMovementScript.MoveY;
        if ((player.transform.position.x < moveBox.xMin) || (player.transform.position.x > moveBox.xMax)) {
            GetComponent<Rigidbody>().AddForce(new Vector2(moveX * moveForce, 0));
            centerX = true;
            if (startHorizontalMove == 0)
                startHorizontalMove = Time.timeSinceLevelLoad;
        }
        if ((player.transform.position.y < moveBox.yMin)|| (player.transform.position.y > moveBox.yMax)) {
            GetComponent<Rigidbody>().AddForce(new Vector2(0, moveY * moveForce));
            centerY = true;
            if (startVerticalMove == 0)
                startVerticalMove = Time.timeSinceLevelLoad;
        }
        GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -maxSpeed, maxSpeed), 0);
        if (centerX && Time.timeSinceLevelLoad - startHorizontalMove > cameraCenterDelay) {
            float playerDistance = player.transform.position.x - transform.position.x;
            GetComponent<Rigidbody>().AddForce(new Vector2(playerDistance * cameraRecenterForce* 5, 0));
        }
        if (centerY && Time.timeSinceLevelLoad - startVerticalMove > cameraCenterDelay) {
            GetComponent<Rigidbody>().AddForce(new Vector2(0, (player.transform.position.y - transform.position.y) * cameraRecenterForce));
        }
        if (player.transform.position.x >= centerBox.xMin && player.transform.position.x <= centerBox.xMax) {
            centerX = false;
            startHorizontalMove = 0f;
        }
        if (player.transform.position.y >= centerBox.yMin && player.transform.position.y <= centerBox.yMax) {
            centerY = false;
            startVerticalMove = 0f;
        }
    }
    
}
