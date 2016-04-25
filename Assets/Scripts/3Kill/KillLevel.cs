using UnityEngine;
using System.Collections;

public class KillLevel : MonoBehaviour {

    public int levelTime;
    public float cameraZoomedIn;
    public float zoomStartSpeed;
    public float zoomStartSpeedAcceleration;
    public float zoomEndSpeed;
    public float zoomEndSpeedAcceleration;
    public float killDisappearTime;
    public float fadeSpeed;
    public float levelEndTime;

    private int timeRemaining;
    private bool endZooming = false;
    private bool startZooming = false;
    private bool fadingIn = false;
    private TextMesh timerText;
    private TextMesh healthText;
    private GameObject killText;
    private Camera thisCamera;
    private Timer killTimer;
    private Timer levelEndTimer;
    private Timer zoomStartTimer;
    private GUITexture fade;
    private GameManager gameManager;
    private PlayerTraits playerTraits;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        playerTraits = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTraits>();
        timerText = GameObject.FindGameObjectWithTag("Time").GetComponent<TextMesh>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TextMesh>();
        killText = GameObject.FindGameObjectWithTag("Eat");
        thisCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
//        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<GUITexture>();
        gameManager.SetLevel(3);
        killText.SetActive(false);
        killTimer = gameObject.AddComponent<Timer>();
        killTimer.Trigger += LiveDisappear;
        levelEndTimer = gameObject.AddComponent<Timer>();
        levelEndTimer.Trigger += ZoomEnd;
        zoomStartTimer = gameObject.AddComponent<Timer>();
        zoomStartTimer.Trigger += ZoomStart;
        zoomStartTimer.Go(1f);
//        fade.color = new Color(0, 0, 0, 1);
        thisCamera.orthographicSize = 0.001f;
    }

    void Update() {

        timeRemaining = (int)(levelTime - Time.timeSinceLevelLoad);
        timerText.text = "Time: " + timeRemaining.ToString();
        if (timeRemaining <= 0 && !startZooming) {
            endZooming = true;
            ZoomEnd();
        }
        
        if (fadingIn)
            FadeIn();
        if (startZooming)
            ZoomStart();
        if (endZooming)
            ZoomEnd();
    }

    void LiveAppear() {

    }

    void LiveDisappear() {
        killText.SetActive(false);
    }

    void ZoomStart() {
        startZooming = true;
        zoomStartSpeed *= zoomStartSpeedAcceleration;
        thisCamera.orthographicSize += zoomStartSpeed * Time.deltaTime;
        if (thisCamera.orthographicSize >= 15) {
            thisCamera.orthographicSize = 15;
            startZooming = false;
            killText.SetActive(true);
            killTimer.Go(killDisappearTime);
            gameManager.StartLevel();
        }
    }

    void ZoomEnd() {
        timerText.text = "";
        healthText.text = "";
        zoomEndSpeed += zoomEndSpeedAcceleration * Time.deltaTime;
        thisCamera.orthographicSize += zoomEndSpeed * Time.deltaTime;
/*        if (thisCamera.orthographicSize > 200) {
            FadeOut();
        }*/
        if (thisCamera.orthographicSize > 300) {
            NextLevel();
        }
    }

    void FadeIn() {
/*        fade.color -= new Color(0, 0, 0, fadeSpeed);
        if (fade.color.a <= 0) {
            fadingIn = false;
            startZooming = true;
        }*/
        fadingIn = false;
        startZooming = true;
    }

/*    void FadeOut() {
        fade.color += new Color(0, 0, 0, fadeSpeed);
    }*/

    void NextLevel() {
        PlayerPrefs.SetInt("PlayerHealth", playerTraits.Health);
        if (playerTraits.EnemiesKilled > playerTraits.AlliesKilled)
            PlayerPrefs.SetInt("HasAllies", 1);
        else
            PlayerPrefs.SetInt("HasAllies", 0);
        
        Application.LoadLevel("4Win");
    }

}
