using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour {

    public float destroyBurningObjectTime;
    public float destroyDeathEffectTime;
    public float booyahTime;

    private GameObject killBurn;
    private GameObject killSplat;
    private GameObject bloodSplat;
    private GameObject ashes;
    private Timer booyahTimer;
    private bool isBurning = false;
    public bool IsBurning {
        get { return isBurning; }
        set { isBurning = value; }
    }
    private bool booyahed;
    private float booyahForce;
    private float booyahRadius;
    private Vector3 booyahPosition;

    void Awake() {
        killBurn = (GameObject)Resources.Load("Effects/KillEffects/KillBurn");
        killSplat = (GameObject)Resources.Load("Effects/KillEffects/KillSplat");
        bloodSplat = (GameObject)Resources.Load("Effects/KillEffects/Blood");
        ashes = (GameObject)Resources.Load("Effects/KillEffects/Ashes");
        booyahTimer = gameObject.AddComponent<Timer>();
        booyahTimer.Trigger += EndBooyah;
    }

    void Update() {
        if (booyahed) {
            DoBooyah();
        }
    }

    public void Booyah(float force, float radius, Vector3 pos) {
        booyahed = true;
        booyahForce = force;
        booyahRadius = radius;
        booyahPosition = pos;
        booyahTimer.Go(booyahTime);
    }

    void DoBooyah() {
        GetComponent<Rigidbody>().AddExplosionForce(booyahForce, booyahPosition, booyahRadius, 0, ForceMode.Force);
    }

    void EndBooyah() {
        booyahed = false;
    }

    /*Attack numbers
     * 1. Fire Dash
     * 2. Teleport
     * 3. Booyah
     * 4. Fast Bash
     */
    public void Hit(int attack) {
        GameObject deathEffect = null;
        switch (attack) {
            case 1:
                deathEffect = killBurn;
                break;
            case 2:
                deathEffect = killSplat;
                break;
            case 3:
                deathEffect = killSplat;
                break;
            case 4:
                deathEffect = killSplat;
                break;
            default:
                deathEffect = killSplat;
                break;
        }
        GameObject myEffect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        myEffect.transform.parent = transform;
        Destroy(myEffect, destroyDeathEffectTime);
        if (attack != 1) {
            GetComponent<Renderer>().enabled = false;
            Vector3 decalPos = transform.position;
            decalPos.z = -0.1356406f;
            Instantiate(bloodSplat, decalPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            Destroy(this.gameObject, destroyDeathEffectTime);
        }
        else {
            isBurning = true;
            StartCoroutine("KillMesh");
            Destroy(this.gameObject, destroyDeathEffectTime);
        }
    }

    IEnumerator KillMesh() {
        yield return new WaitForSeconds(destroyBurningObjectTime);
        GetComponent<Renderer>().enabled = false;
        Vector3 decalPos = transform.position;
        decalPos.z = -0.1356406f;
        Instantiate(ashes, decalPos, Quaternion.Euler(0,0,Random.Range(0f,360f)));
    }

}
