using UnityEngine;
using System.Collections;

public class PlayerWin : MonoBehaviour {

    public GameObject allBros;
    public float broRadiusMin;
    public float broRadiusMax;
    public GameObject[] myBros = new GameObject[6];
    public float specialDelay;
    public float explosionRadius;
    public float explosionForce;
    public float quakeRadius;
    public float laserRadius;
    public float laserSpeed;
    public float flameSpeed;
    public float beatingSpeed;

    private bool bumping = false;
    private int damage;
    private float dashForce;
    private float dashTime;
    private float bashForce;
    private float bashTime;
    private bool dashing = false;
    private bool bashing = false;
    private Vector2 clickVector;
    private bool canSpecial = true;
    private bool hasBros = false;
    private bool carnivore = false;
    private bool killer = false;
    private GameObject bizarro;
    private PlayerTraits playerTraits;
    private PlayerMovement playerMove;
    private GameManager gameManager;
    private GameObject teleportEffect;
    private CameraShake cameraShake;
    private Timer specialTimer;
    private Timer dashTimer;
    private Vector3 bumpForce;
    private Timer bumpTimer;

    private bool slime = false;
    private bool comet = false;
    private bool quake = false;
    private bool blackhole = false;
    private bool shield = false;
    private bool lasers = false;
    private bool flamethrower = false;
    private bool beating = false;

    public GameObject slimeEffect;
    private GameObject cometEffect;
    private GameObject explosion;
    private GameObject quakeEffect;
    private GameObject bloodEffect;

    private GameObject beatingEffect;

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, broRadiusMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, broRadiusMax);
    }

    void Awake() {
        playerTraits = GetComponent<PlayerTraits>();
        playerMove = GetComponent<PlayerMovement>();
        bizarro = GameObject.FindGameObjectWithTag("Bizarro");
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        teleportEffect = (GameObject)Resources.Load("Effects/KillEffects/Teleport");
        cometEffect = (GameObject)Resources.Load("Effects/Special/Comet");
        explosion = (GameObject)Resources.Load("Effects/Special/Explosion");
        quakeEffect = (GameObject)Resources.Load("Effects/Special/Quake");
        bloodEffect = (GameObject)Resources.Load("Effects/Special/MobSplat");
        beatingEffect = (GameObject)Resources.Load("Effects/Special/MobBeating");
        specialTimer = gameObject.AddComponent<Timer>();
        bumpTimer = gameObject.AddComponent<Timer>();
        bumpTimer.Trigger += BumpOff;
        damage = playerMove.damage;
        specialTimer.Trigger += SpecialClear;
        dashForce = playerMove.dashForce * 2;
        dashTime = playerMove.dashTime;
        bashForce = playerMove.bashForce;
        bashTime = playerMove.bashTime;
        dashTimer = gameObject.AddComponent<Timer>();
        dashTimer.Trigger += EndDash;

    }


    void Update() {
        if (gameManager.LevelStarted) {
            if (Input.GetButtonDown("Fire2") && canSpecial) {
                clickVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                canSpecial = false;
                specialTimer.Go(specialDelay);
                if (slime) {
                    GameObject mySlime = (GameObject)Instantiate(slimeEffect, transform.position, Quaternion.Euler(0, 0, 0));
                    Destroy(mySlime, 15f);
                }
                else if (comet) {
                    GameObject myComet = (GameObject)Instantiate(cometEffect, transform.position, Quaternion.identity);
                    myComet.transform.parent = transform;
                    dashing = true;
                    dashTimer.Go(dashTime);
                    Destroy(myComet, 5f);
                }
                else if (quake) {
                    GameObject myQuake = (GameObject)Instantiate(quakeEffect, transform.position, Quaternion.Euler(0,180,0));
                    Destroy(myQuake, 10f);
                    cameraShake.ShakeMe(3f);
                    bizarro.GetComponent<Bizarro>().DazeMe(3f) ;
                }
                else if (blackhole) {
                    gameManager.GetComponent<WinLevel>().BlackHole(clickVector);
                }
                else if (lasers) {
                    if (Vector3.Distance(transform.position, bizarro.transform.position) <= laserRadius) {
                        if (hasBros) {
                            allBros.GetComponent<AllBros>().Laser(clickVector);
                        }
                    }
                }
                else if (flamethrower) {
                    if (hasBros) {
                        allBros.GetComponent<AllBros>().Flames(clickVector);
                    }
                }
                else if (beating) {
                    if (hasBros) {
                        allBros.GetComponent<AllBros>().Beating(clickVector);
                    }
                }
            }
            if (Input.GetButtonDown("Fire2") && shield) {
                if (hasBros) {
                    for (int i = 0; i < myBros.Length; i++) {
                        myBros[i].GetComponent<Bro>().shielding = true;
                    }
                }
            }
            else if (Input.GetButtonUp("Fire2") && shield) {
                if (hasBros) {
                    for (int i = 0; i < myBros.Length; i++) {
                        myBros[i].GetComponent<Bro>().shielding = false;
                    }
                }
            }
        }
    }

    void FixedUpdate() {
        if (dashing)
            Dash();
        if (bumping)
            Bumping();
    }


    void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("EvilShield")) {
            coll.gameObject.transform.parent.gameObject.GetComponent<Bro>().Kill();
            Vector3 thisBumpVector = coll.contacts[0].normal;
            if (thisBumpVector.magnitude != 0)
                thisBumpVector /= thisBumpVector.magnitude;
            Hit(10, thisBumpVector * 1500);
        }
        if (coll.gameObject.CompareTag("EvilLaser") || coll.gameObject.CompareTag("Fire")) {
            playerTraits.Hit(10);
            coll.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        }
        if (coll.gameObject.CompareTag("EvilBeat")) {
            playerTraits.Hit(30);
            GameObject blood = (GameObject)Instantiate(bloodEffect, coll.transform.position, Quaternion.identity);
            Destroy(blood, 5f);
            Destroy(coll.gameObject);
        }
    }

    public void Hit(int damage, Vector3 bForce) {
        playerMove.Dazed = true;
        playerTraits.Hit(damage);
        bumping = true;
        bumpTimer.Go(0.2f);
        bumpForce = bForce;

    }

    void BumpOff() {
        bumping = false;
    }

    void Bumping() {
        GetComponent<Rigidbody>().AddForce(bumpForce);
    }

    void SpecialClear() {
        canSpecial = true;
        allBros.SetActive(true);
    }

    void Dash() {
        Vector2 dashVector = clickVector;
        if (dashVector.magnitude != 0)
            GetComponent<Rigidbody>().AddForce(dashVector / dashVector.magnitude * dashForce);
    }

    void EndDash() {
        bashing = false;
        dashing = false;
        if (comet) {
            GameObject exp = (GameObject)Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(exp, 2f);
            Boom(explosionForce, explosionRadius);
        }

    }

    void Boom(float boomForce, float boomRadius) {
        Collider[] moreColliders = Physics.OverlapSphere(transform.position, 2*boomRadius);
        foreach (Collider c in moreColliders) {
            if (c.gameObject.CompareTag("Bizarro")) {
                Vector3 boomVector = c.gameObject.transform.position - transform.position;
                if (boomVector.magnitude != 0)
                    boomVector /= boomVector.magnitude;
                c.GetComponent<Bizarro>().Hit(damage, boomVector * boomForce);
            }
        }
        cameraShake.ShakeMe(0.5f);
    }

    public void TeleportBros(Vector3 oldPos) {
        if (hasBros) {
            for (int i = 0; i < myBros.Length; i++) {
                GameObject tpEffect = (GameObject)Instantiate(teleportEffect, myBros[i].transform.position + myBros[i].GetComponent<Bro>().MyOffset, Quaternion.Euler(0, 0, 180));
            }
        }
    }

    public void SetTraits(bool meat, bool killah, bool bros) {
        hasBros = bros;
        carnivore = meat;
        killer = killah;
        if (!hasBros) {
            for (int i = 0; i < myBros.Length; i++) {
                myBros[i].SetActive(false);
            }
        }
        if (carnivore && !killer && !hasBros)
            slime = true;
        else if (!carnivore && killer && !hasBros)
            comet = true;
        else if (carnivore && killer && !hasBros)
            quake = true;
        else if (!carnivore && !killer && !hasBros)
            blackhole = true;
        else if (carnivore && killer && hasBros)
            shield = true;
        else if (!carnivore && !killer & hasBros)
            lasers = true;
        else if (!carnivore && killer & hasBros)
            flamethrower = true;
        else if (carnivore && !killer & hasBros)
            beating = true;
    }


}
