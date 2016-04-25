using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float moveForce;
    public float maxSpeed;
    public float bashTime;
    public float bashResetTime;
    public float bashForce;
    public float bashHitForce;
    public float dashTime;
    public float dashResetTime;
    public float dashForce;
    public float bumpTime;
    public float teleportDistance;
    public float telefragRadius;
    public float booyahKillRadius;
    public float booyahShockwaveRadius;
    public float booyahShockwaveForce;
    public int damage;

    private bool dazed = false;
    public bool Dazed {
        get { return dazed; }
        set { 
            dazed = value;
            if (dazed)
                dazeTimer.Go(dazeTime);
        }
    }
    public float dazeTime ;
    private Timer dazeTimer;



    private float moveX;
    public float MoveX {
        get { return moveX; }
    }
    private float moveY;
    public float MoveY {
        get { return moveY; }
    }

    private bool killer = false;
    private bool canTeleport = false;
    private bool dash = false;
    private bool dashing = false;
    public bool Dashing {
        get { return dashing; }
    }
    private bool canDash = true;
    private bool bash = false;    
    private bool bashing = false;
    public bool Bashing {
        get { return bashing; }
    }
    private bool canBash = true;
    private bool bumpingGigantor = false;
    private bool afterglow = false;
    private bool hit = false;
    public bool Afterglow {
        get { return afterglow; }
    }
    private Vector3 bumpVector;
    private Vector2 clickVector;
    private Timer dashTimer;
    private Timer dashReset;
    private Timer bashTimer;
    private Timer bashReset;
    private Timer bumpTimer;
    private Timer afterglowTimer;
    private GameManager gameManager;
    private GameObject gigantor;
    private PlayerTraits playerTraits;
    private PlayerKill playerKill;


    //prefabs
    private GameObject dashEffect;
    private GameObject bashEffect;
    private GameObject endTeleportEffect;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        playerTraits = GetComponent<PlayerTraits>();
        playerKill = GetComponent<PlayerKill>();
        dashEffect = (GameObject)Resources.Load("Effects/Dash");
        bashEffect = (GameObject)Resources.Load("Effects/Bash");
        bumpTimer = gameObject.AddComponent<Timer>();
        bumpTimer.Trigger += BumpOff;
        afterglowTimer = gameObject.AddComponent<Timer>();
        afterglowTimer.Trigger += AfterglowOff;
        dazeTimer = gameObject.AddComponent<Timer>();
        dazeTimer.Trigger += DazeOff;
    }

    void Start() { 

    }

    void Update() {
        if (gameManager.LevelStarted && !dazed) {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
            if (Input.GetButton("Fire1")) {
                clickVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                if (bash && canBash) {
                    bashing = true;
                    if(gameManager.CurrentLevel >= 3)
                        playerKill.Attacking = true;
                    afterglow = true;
                    canBash = false;
                    bashTimer.Go(bashTime);
                    bashReset.Go(bashResetTime);
                    GameObject myBashEffect = (GameObject)Instantiate(bashEffect, transform.position, Quaternion.Euler(0, -180, 180));
                    myBashEffect.transform.parent = transform;
                    Destroy(myBashEffect, 5f);

                }
                else if (dash && canDash) {
                    canDash = false;
                    dashTimer.Go(dashTime);
                    dashReset.Go(dashResetTime);
                    if (!canTeleport) {
                        dashing = true;
                        if (gameManager.CurrentLevel >= 3)
                            playerKill.Attacking = true;
                        GameObject myDashEffect = (GameObject)Instantiate(dashEffect, transform.position, Quaternion.Euler(0, 180, 180));
                        myDashEffect.transform.parent = transform;
                        Destroy(myDashEffect, 5f);
                    }
                    else {
                        Vector3 newPos = transform.position;
                        float teleportMagnitude = clickVector.magnitude;
                        if (teleportMagnitude != 0) {
                            if (gameManager.CurrentLevel == 4) {
                                GetComponent<PlayerWin>().TeleportBros(transform.position);
                            }
                            GameObject startTeleport = (GameObject)Instantiate(dashEffect, transform.position, Quaternion.Euler(0, 0, 180));
                            Destroy(startTeleport, 1f);
                            if (Vector3.Distance(transform.position, clickVector) >= teleportDistance) {
                                //clickVector = Vector3.Normalize(clickVector);
                                newPos.x += clickVector.x * teleportDistance / teleportMagnitude;
                                newPos.y += clickVector.y * teleportDistance / teleportMagnitude;
                            }
                            else
                                newPos = clickVector;
                            transform.position = newPos;
                            GameObject endTeleport = (GameObject)Instantiate(endTeleportEffect, transform.position, Quaternion.Euler(0, 180, 180));
                            Destroy(endTeleport, 5f);

                            Collider[] colliders = Physics.OverlapSphere(transform.position, telefragRadius);
                            foreach (Collider c in colliders) {
                                if (c.gameObject.CompareTag("Enemy")) {
                                    playerTraits.EnemiesKilled++;
                                    playerTraits.AddHealth(2);
                                    c.GetComponent<Death>().Hit(2);

                                }
                                else if (c.gameObject.CompareTag("Ally")) {
                                    playerTraits.AlliesKilled++;
                                    playerTraits.AddHealth(1);
                                    c.GetComponent<Death>().Hit(2);
                                }
                            }
                        }
                    }
                }
            }
            if (bashing)
                Bash();
            else if (dashing)
                Dash();
        }
        if (bumpingGigantor)
            BumpGigantor();
    }

    void DazeOff() {
        dazed = false;
    }

    void AfterglowOff() {
        afterglow = false;
    }

    void Bash() {
        Vector2 bashVector = clickVector;
        if (bashVector.magnitude != 0)
            GetComponent<Rigidbody>().AddForce(bashVector / bashVector.magnitude * bashForce);
    }

    void EndBash() {
        bashing = false;
        if (gameManager.CurrentLevel >= 3) {
            playerKill.Attacking = false;
            Booyah();
        }
        if (hit)
            bumpingGigantor = true;
        bumpTimer.Go(bumpTime);
        afterglowTimer.Go(bashTime / 2);
        hit = false;
    }

    void Booyah() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, booyahKillRadius);
        foreach (Collider c in colliders) {
            if (c.gameObject.CompareTag("Enemy")) {
                playerTraits.EnemiesKilled++;
                playerTraits.AddHealth(3);
                c.GetComponent<Death>().Hit(3);

            }
            else if (c.gameObject.CompareTag("Ally")) {
                playerTraits.AlliesKilled++;
                playerTraits.AddHealth(1);
                c.GetComponent<Death>().Hit(3);
            }
        }

        Collider[] moreColliders = Physics.OverlapSphere(transform.position, booyahShockwaveRadius);
        foreach (Collider c in moreColliders) {
            if (c.gameObject.CompareTag("Enemy") || c.gameObject.CompareTag("Ally")) {
                c.GetComponent<Death>().Booyah(booyahShockwaveForce, booyahShockwaveRadius, transform.position);
            }
        }
    }

    void CanBash() {
        canBash = true;
    }

    void Dash() {
        Vector2 dashVector = clickVector;
        if (dashVector.magnitude != 0)
            GetComponent<Rigidbody>().AddForce(dashVector / dashVector.magnitude * dashForce);
    }

    void EndDash() {
        dashing = false;
        if (gameManager.CurrentLevel >= 3)
            playerKill.Attacking = false;
    }

    void CanDash() {
        canDash = true;
    }

    void FixedUpdate() {
        GetComponent<Rigidbody>().AddForce(new Vector2(moveX * moveForce, moveY * moveForce));
        GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(GetComponent<Rigidbody>().velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, -maxSpeed, maxSpeed), 0);
    }

    void OnCollisionEnter(Collision coll) {
        if (bashing && coll.gameObject.CompareTag("Gigantor")) {
            gigantor = coll.gameObject;
            bumpVector = (-coll.contacts[0].normal * bashHitForce);
            hit = true;
        }
        if (bashing && coll.gameObject.CompareTag("Bizarro")) {
            coll.gameObject.GetComponent<Bizarro>().Hit(damage, -coll.contacts[0].normal * bashHitForce);
        }
    }

    public void Bump(Vector3 bumpForce) {
        GetComponent<Rigidbody>().AddForce(bumpForce);
    }

    void BumpGigantor() {
        gigantor.GetComponent<GigantorMovement>().BumpMe(bumpVector);
    }

    void BumpOff() {
        bumpingGigantor = false;
    }

    public void SetPowers() {

        if (playerTraits.Carnivore) {
            bash = true;
            bashTimer = gameObject.AddComponent<Timer>();
            bashTimer.Trigger += EndBash;
            bashReset = gameObject.AddComponent<Timer>();
            bashReset.Trigger += CanBash;
        }
        else {
            dash = true;
            dashTimer = gameObject.AddComponent<Timer>();
            dashTimer.Trigger += EndDash;
            dashReset = gameObject.AddComponent<Timer>();
            dashReset.Trigger += CanDash;
        }
        if (gameManager.CurrentLevel >= 3) {
            if (playerTraits.Killer) {
                killer = true;
                dashEffect = (GameObject)Resources.Load("Effects/KillEffects/FireDash");
                bashEffect = (GameObject)Resources.Load("Effects/KillEffects/Booyah");
                bashHitForce *= 2;
            }
            else {
                killer = false;
                if (gameManager.CurrentLevel >= 3)
                    dashEffect = (GameObject)Resources.Load("Effects/KillEffects/Teleport");
                endTeleportEffect = (GameObject)Resources.Load("Effects/KillEffects/TeleportEnd");
                bashEffect = (GameObject)Resources.Load("Effects/KillEffects/LongBash");
                bashTime += 0.2f;
                bashForce += 200;
            }
            if (!playerTraits.Carnivore && !playerTraits.Killer && gameManager.CurrentLevel >= 3)
                canTeleport = true;
            if (gameManager.CurrentLevel == 3)
                GetComponent<PlayerKill>().SetTraits(bash, killer);
        }
    }
}
