using UnityEngine;
using System.Collections;

public class BizarroAttack : MonoBehaviour {


    public GameObject allBros;
    public GameObject[] myBros = new GameObject[6];
    public float specialDelay;
    public float explosionRadius;
    public float explosionForce;
    public float quakeRadius;
    public float laserRadius;
    public float laserSpeed;
    public float flameSpeed;
    public float beatingSpeed;

    public int damage;
    public float dashForce;
    public float bashForce;
    private bool dashing = false;
    private bool bashing = false;
    private bool canSpecial = false;
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
    private GameObject player;
    private Vector3 clickVector;



    private bool canTeleport = false;
    private bool canBash = false;
    private bool canDash = false;
    private bool dash = false;
    private bool bash = false;
    private Timer bashTimer;
    private Timer bashReset;
    private Timer dashReset;
    public float bashTime;
    public float dashTime;
    public float bashResetTime;
    public float dashResetTime;
    public float bashHitForce;
    private GameObject bashEffect;
    private GameObject dashEffect;
    private float teleportDistance;
    private GameObject endTeleportEffect;




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

    private GameObject beatingEffect;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTraits = player.GetComponent<PlayerTraits>();
        playerMove = player.GetComponent<PlayerMovement>();
        bizarro = GameObject.FindGameObjectWithTag("Bizarro");
        
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        teleportEffect = (GameObject)Resources.Load("Effects/KillEffects/Teleport");
        cometEffect = (GameObject)Resources.Load("Effects/Special/Comet");
        explosion = (GameObject)Resources.Load("Effects/Special/Explosion");
        quakeEffect = (GameObject)Resources.Load("Effects/Special/Quake");
        dashEffect = (GameObject)Resources.Load("Effects/Dash");
        bashEffect = (GameObject)Resources.Load("Effects/Bash");

        beatingEffect = (GameObject)Resources.Load("Effects/Special/MobBeating");
        specialTimer = gameObject.AddComponent<Timer>();
        specialTimer.Trigger += SpecialClear;
        specialTimer.Go(3f);
        dashTimer = gameObject.AddComponent<Timer>();
        dashTimer.Trigger += EndDash;
        bashTimer = gameObject.AddComponent<Timer>();
        bashTimer.Trigger += EndDash;
        bashReset = gameObject.AddComponent<Timer>();
        bashReset.Trigger += CanBash;
        dashReset = gameObject.AddComponent<Timer>();
        dashReset.Trigger += CanDash;

    }



    void Update() {
        if (gameManager.LevelStarted) {
            if (canSpecial) {
                clickVector = player.transform.position;
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
                    GameObject myQuake = (GameObject)Instantiate(quakeEffect, transform.position, Quaternion.Euler(0, 180, 0));
                    Destroy(myQuake, 10f);
                    cameraShake.ShakeMe(3f);
                    playerMove.Dazed = true;
                }
                else if (lasers) {
                    if (Vector3.Distance(transform.position, bizarro.transform.position) <= laserRadius) {
                        if (hasBros) {
                            allBros.GetComponent<EvilAllBros>().Laser(clickVector);
                        }
                    }
                }
                else if (flamethrower) {
                    if (hasBros) {
                        allBros.GetComponent<EvilAllBros>().Flames(clickVector);
                    }
                }
                else if (beating) {
                    if (hasBros) {
                        allBros.GetComponent<EvilAllBros>().Beating(clickVector);
                    }
                }
            }
            if (shield) {
                if (hasBros) {
                    for (int i = 0; i < myBros.Length; i++) {
                        myBros[i].GetComponent<EvilBro>().shielding = true;
                    }
                }
            }






            if (canBash || canDash) {
                clickVector = player.transform.position - transform.position;
                if (bash && canBash) {
                    bashing = true;
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
                        GameObject myDashEffect = (GameObject)Instantiate(dashEffect, transform.position, Quaternion.Euler(0, 180, 180));
                        myDashEffect.transform.parent = transform;
                        Destroy(myDashEffect, 5f);
                    }
                    else {
                        Vector3 newPos = transform.position;
                        float teleportMagnitude = clickVector.magnitude;
                        if (teleportMagnitude != 0) {
                            if (gameManager.CurrentLevel == 4) {
                                TeleportBros(transform.position);
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
                        }
                    }
                }
            }

        }

    }


    void TeleportBros(Vector3 oldPos) {
        if (hasBros) {
            for (int i = 0; i < myBros.Length; i++) {
                GameObject tpEffect = (GameObject)Instantiate(teleportEffect, myBros[i].transform.position + myBros[i].GetComponent<EvilBro>().MyOffset, Quaternion.Euler(0, 0, 180));
            }
        }
    }


    void FixedUpdate() {
        if (dashing)
            Dash();
        else if (bashing)
            Bash();
    }


    void CanDash() {
        canDash = true;
    }


    void CanBash() {
        canBash = true;
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

    void Bash() {
        Vector2 bashVector = clickVector;
        if (bashVector.magnitude != 0)
            GetComponent<Rigidbody>().AddForce(bashVector / bashVector.magnitude * bashForce);
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
        Collider[] moreColliders = Physics.OverlapSphere(transform.position, 2 * boomRadius);
        foreach (Collider c in moreColliders) {
            if (c.gameObject.CompareTag("Player")) {
                Vector3 boomVector = c.gameObject.transform.position - transform.position;
                if (boomVector.magnitude != 0)
                    boomVector /= boomVector.magnitude;
                c.GetComponent<Bizarro>().Hit(damage, boomVector * boomForce);
            }
        }
        cameraShake.ShakeMe(0.5f);
    }

    public void SetTraits(bool meat, bool killah, bool bros) {
        hasBros = !bros;
        carnivore = !meat;
        killer = !killah;
        if (!hasBros) {
            for (int i = 0; i < myBros.Length; i++) {
                myBros[i].SetActive(false);
            }
        }

        if (carnivore) {
            bash = true;
            canBash = true;
        }
        else {
            dash = true;
            canDash = true;
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

        if (killer) {
            killer = true;
            dashEffect = (GameObject)Resources.Load("Effects/KillEffects/FireDash");
            bashEffect = (GameObject)Resources.Load("Effects/KillEffects/Booyah");
            bashHitForce *= 2;
        }
        else {
            killer = false;
            canTeleport = false;
            dash = false;
            canDash = false;
            dashEffect = (GameObject)Resources.Load("Effects/KillEffects/Teleport");
            endTeleportEffect = (GameObject)Resources.Load("Effects/KillEffects/TeleportEnd");
            bashEffect = (GameObject)Resources.Load("Effects/KillEffects/LongBash");
            bashTime += 0.2f;
            bashForce += 200;
        }


    }


}
