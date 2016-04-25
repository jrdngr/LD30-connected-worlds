using UnityEngine;
using System.Collections;

public class OtherHealth : MonoBehaviour {

    //Inspector stuff
    public int healthValue;
    public float flashTime;
    
    //Health stuff
    private int health = 30;
    public int Health {
        get { return health; }
    }
    private bool dying = false;

    //Shrink animation
    public float shrinkSpeed;
    public float shrinkForce;
    private Vector3 startScale;
    private float shrinkStartTime = 0f;    
    
    private Timer flashTimer;
    private GameObject player;
    private PlayerTraits playerTraits;
    private EatLevel gameManager;
    private float shrinkDistance;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<EatLevel>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTraits = player.GetComponent<PlayerTraits>();
        flashTimer = gameObject.AddComponent<Timer>();
        flashTimer.Trigger += FlashBack;
        startScale = transform.localScale;
        shrinkDistance = Vector3.Distance(startScale, Vector3.zero);
    }

    void FixedUpdate() {
        if (dying)
            Death();
    }

    void Death() {
        GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position) * shrinkForce);
        float timeFactor = (Time.time - shrinkStartTime) * shrinkSpeed;
        transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timeFactor);
    }

    void FlashBack() {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void Hit(int damage, Vector3 normal, float bumpForce) {
        health -= damage;
        GetComponent<Rigidbody>().AddForce(-normal * bumpForce);
        GetComponent<Renderer>().material.color = Color.red;
        flashTimer.Go(flashTime);
        if (health <= 0) {
            playerTraits.AddHealth(healthValue);
            playerTraits.OthersEaten++;
            GetComponent<Renderer>().material.color = Color.red;
            Destroy(this.gameObject, 2f);
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().detectCollisions = false;
            dying = true;
            shrinkStartTime = Time.time;
            gameManager.EnemyFear++;
        }
    }

}
