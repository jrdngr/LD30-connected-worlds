using UnityEngine;
using System.Collections;

public class LiveLevel : MonoBehaviour {

    public float cameraZoomedIn;
    public float zoomStartSpeed;
    public float zoomStartSpeedAcceleration;
    public float zoomEndSpeed;
    public float zoomEndSpeedAcceleration;
    public float liveDisappearTime;
    public float fadeSpeed;
    public float levelEndTime;

    private bool endZooming = false;
    private bool startZooming = false;
    private bool fadingIn = false;
    private bool triggeredEnd = false;
    private TextMesh timerText;
    private TextMesh healthText;
    private GameObject liveText;
    private Camera thisCamera;
    private Timer liveTimer;
    private Timer levelEndTimer;
    private Timer startZoomTimer;
    private GUITexture fade;
    private GameManager gameManager;
    private PlayerTraits playerTraits;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        playerTraits = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTraits>();
        timerText = GameObject.FindGameObjectWithTag("Time").GetComponent<TextMesh>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TextMesh>();
        liveText = GameObject.FindGameObjectWithTag("Eat");
        thisCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
//        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<GUITexture>();
        gameManager.SetLevel(2);
        timerText.text = "";
        liveText.SetActive(false);
        liveTimer = gameObject.AddComponent<Timer>();
        liveTimer.Trigger += LiveDisappear;
        levelEndTimer = gameObject.AddComponent<Timer>();
        levelEndTimer.Trigger += ZoomEnd;
        startZoomTimer = gameObject.AddComponent<Timer>();
        startZoomTimer.Trigger += ZoomStart;
        startZoomTimer.Go(1f);
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
    }

    void LiveAppear() {

    }

    void LiveDisappear() {
        liveText.SetActive(false);
    }

    void ZoomStart() {
        startZooming = true;
        zoomStartSpeed *= zoomStartSpeedAcceleration;
        thisCamera.orthographicSize += zoomStartSpeed * Time.deltaTime;
        if (thisCamera.orthographicSize >= 15) {
            thisCamera.orthographicSize = 15;
            startZooming = false;
            liveText.SetActive(true);
            liveTimer.Go(liveDisappearTime);
            gameManager.StartLevel();
        }
    }

    void ZoomEnd() {
        timerText.text = "";
        healthText.text = "";
        endZooming = true;
        zoomEndSpeed += zoomEndSpeedAcceleration * Time.deltaTime;
        thisCamera.orthographicSize += zoomEndSpeed * Time.deltaTime;
/*        if (thisCamera.orthographicSize > 600) {
            FadeOut();
        }*/
        if (thisCamera.orthographicSize > 700) {
            NextLevel();
        }
    }

/*    void FadeIn() {
        fade.color -= new Color(0, 0, 0, fadeSpeed);
        if (fade.color.a <= 0) {
            fadingIn = false;
            startZooming = true;
        }
    }

    void FadeOut() {
        fade.color += new Color(0, 0, 0, fadeSpeed);
    }*/

    void NextLevel() {
        Application.LoadLevel("3Kill");
    }

    public void EndLevel(bool killed) {
        if (!triggeredEnd) {
            triggeredEnd = true;
            PlayerPrefs.SetInt("Health", playerTraits.Health);
            if (killed) {
                PlayerPrefs.SetInt("Killer", 1);
                levelEndTimer.Go(levelEndTime);
            }
            else {
                PlayerPrefs.SetInt("Killer", 0);
                ZoomEnd();
            }
        }
        
    }

    public void YouDied() {
        StartCoroutine("YouJustDied");
    }

    IEnumerator YouJustDied() {
        yield return new WaitForSeconds(2f);
        Application.LoadLevel("Game Over");
    }

}
