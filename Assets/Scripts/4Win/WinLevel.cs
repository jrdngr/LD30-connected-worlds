using UnityEngine;
using System.Collections;

public class WinLevel : MonoBehaviour {

    public float cameraZoomedIn;
    public float zoomStartSpeed;
    public float zoomStartSpeedAcceleration;
    public float zoomEndSpeed;
    public float zoomEndSpeedAcceleration;
    public float winDisappearTime;
    public float fadeSpeed;
    public float choiceDelay;
    public GameObject healthDisplay;

    public float blackHoleForce;
    public float blackHoleRadius;
    public float bgShrinkSpeed;
    public float bgShrinkAccel;
    public GameObject background;

    private bool endZooming = false;
    private bool startZooming = false;
    private bool fadingIn = false;
    private bool showingChoices = false;
    private string eatChoice;
    private string liveChoice;
    private string killChoice;
    private TextMesh timerText;
    private TextMesh healthText;
    private TextMesh eatText;
    private TextMesh liveText;
    private TextMesh killText;
    private GameObject winText;
    private Camera thisCamera;
    private Timer winTimer;
    private Timer zoomStartTimer;
    private Timer eatTextTimer;
    private Timer liveTextTimer;
    private Timer killTextTimer;
//    private GUITexture fade;
    private GameManager gameManager;
    private PlayerTraits playerTraits;
    private Bizarro bizarro;
    private GameObject blackHoleEffect;

    private bool blackHoling = false;
    private Vector3 blackHolePos;



    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        playerTraits = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTraits>();
        timerText = GameObject.FindGameObjectWithTag("Time").GetComponent<TextMesh>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TextMesh>();
        winText = GameObject.FindGameObjectWithTag("Eat");
        thisCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        eatText = GameObject.FindGameObjectWithTag("ET").GetComponent<TextMesh>();
        liveText = GameObject.FindGameObjectWithTag("LT").GetComponent<TextMesh>();
        killText = GameObject.FindGameObjectWithTag("KT").GetComponent<TextMesh>();
        bizarro = GameObject.FindGameObjectWithTag("Bizarro").GetComponent<Bizarro>();
 //       fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<GUITexture>();
        timerText.text = "";
        blackHoleEffect = (GameObject)Resources.Load("Effects/Special/BlackHole");
        gameManager.SetLevel(4);
        winText.SetActive(false);
        winTimer = gameObject.AddComponent<Timer>();
        winTimer.Trigger += WinDisappear;
        zoomStartTimer = gameObject.AddComponent<Timer>();
        zoomStartTimer.Trigger += ZoomStart;
        
        zoomStartTimer.Go(1f);
       // startZooming = true;
        //DEBUGGING

        eatTextTimer = gameObject.AddComponent<Timer>();
        eatTextTimer.Trigger += GoLive;
        liveTextTimer = gameObject.AddComponent<Timer>();
        liveTextTimer.Trigger += GoKill;
        killTextTimer = gameObject.AddComponent<Timer>();
        killTextTimer.Trigger += GoWin;

//        fade.color = new Color(0, 0, 0, 1);
        thisCamera.orthographicSize = 0.001f;
    }

    void Update() {
/*        if (fadingIn)
            FadeIn();*/
        if (startZooming)
            ZoomStart();
        if (endZooming)
            ZoomEnd();
        if (blackHoling){
            Suck();
        }
    }

    public void Choices() {
        showingChoices = true;
        eatChoice = (playerTraits.Carnivore) ? "MEAT!" : "PLANTS!";
        liveChoice = (playerTraits.Killer) ? "MURDER!" : "ESCAPE";
        killChoice = (playerTraits.HasAllies) ? "FRIENDSHIP!" : "MORAL RELATIVISM!";
    }

    void WinDisappear() {
        winText.SetActive(false);
    }

    void GoLive(){
        liveText.text = liveChoice;
        liveTextTimer.Go(choiceDelay);
    }

    void GoKill(){
        killText.text = killChoice;
        killTextTimer.Go(choiceDelay);
    }

    void GoWin(){
        
        eatText.gameObject.SetActive(false);
        liveText.gameObject.SetActive(false);
        killText.gameObject.SetActive(false);
        winText.gameObject.SetActive(true);
        winTimer.Go(winDisappearTime);
        bizarro.TeleportIn();
        gameManager.StartLevel();
    }

    void ZoomStart() {
        startZooming = true;
        zoomStartSpeed *= zoomStartSpeedAcceleration;
        thisCamera.orthographicSize += zoomStartSpeed * Time.deltaTime;
        if (thisCamera.orthographicSize >= 15) {
            thisCamera.orthographicSize = 15;
            startZooming = false;
            eatText.text = eatChoice;
            eatTextTimer.Go(choiceDelay);
            //GoWin();
        }
        
    }

    void ZoomEnd() {
        zoomEndSpeed += zoomEndSpeedAcceleration * Time.deltaTime;
        thisCamera.orthographicSize += zoomEndSpeed * Time.deltaTime;
/*       if (thisCamera.orthographicSize > 200) {
            FadeOut();
        }*/
        if (thisCamera.orthographicSize > 300) {
            NextLevel();
        }
    }

 /*   void FadeIn() {
        fade.color -= new Color(0, 0, 0, fadeSpeed);
        if (fade.color.a <= 0) {
            fadingIn = false;
            startZooming = true;
        }
    }

   void FadeOut() {
        fade.color += new Color(0, 0, 0, fadeSpeed);
    }*/

    void Suck() {
        Collider[] colliders = Physics.OverlapSphere(blackHolePos, blackHoleRadius);
        foreach (Collider c in colliders) {
            if (c.GetComponent<Rigidbody>() && (c.GetComponent<Rigidbody>().CompareTag("Player") || c.GetComponent<Rigidbody>().CompareTag("Bizarro"))) {
                c.GetComponent<Rigidbody>().AddExplosionForce(-blackHoleForce, blackHolePos, blackHoleRadius, 0, ForceMode.Force);
            }
        }
        background.transform.position = new Vector3(blackHolePos.x, blackHolePos.y, 0);
        background.transform.localScale -= new Vector3(bgShrinkSpeed, bgShrinkSpeed, bgShrinkSpeed);
        bgShrinkSpeed *= bgShrinkAccel;
        if (background.transform.localScale.x <= 0) {
            Destroy(background.gameObject);
            blackHoling = false;
            StartCoroutine("YouSuck");
        }
    }

    public void BlackHole(Vector3 click) {
        thisCamera.orthographicSize = 40;
        blackHolePos = click;
        blackHolePos.z = -30;
        Instantiate(blackHoleEffect, blackHolePos, Quaternion.identity);
        gameManager.BlackHoled();
        healthDisplay.SetActive(false);
        blackHoling = true;
    }

    public void NextLevel() {
        StartCoroutine("EndDelay");
    }

    public void YouDied() {
        StartCoroutine("YouJustDied");
    }

    IEnumerator EndDelay() {
        yield return new WaitForSeconds (3f);
        Application.LoadLevel("You Win");
    }

    IEnumerator YouSuck() {
        yield return new WaitForSeconds(5f);
        Application.LoadLevel("Black Hole");
    }

    IEnumerator YouJustDied() {
        yield return new WaitForSeconds(2f);
        Application.LoadLevel("Game Over");
    }

}
