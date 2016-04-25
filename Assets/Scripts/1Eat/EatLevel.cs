using UnityEngine;
using System.Collections;

public class EatLevel : MonoBehaviour {

    private int enemyFear = 0;
    public int EnemyFear {
        get { return enemyFear; }
        set { enemyFear = value; }
    }

    public int levelTime;
    public float cameraZoomedIn;
    public float zoomSpeed;
    public float zoomSpeedAcceleration;
    public float eatDisappearTime;
    public float fadeSpeed;

    private int timeRemaining;
    private bool zooming = false;
    private TextMesh timerText;
    private TextMesh healthText;
    private GameObject eatText;
    private Camera thisCamera;
    private Timer eatTimer;
    private Timer startTimer;
    private GUITexture fade;
    private PlayerTraits playerTraits;
    private GameManager gameManager;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        timerText = GameObject.FindGameObjectWithTag("Time").GetComponent<TextMesh>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TextMesh>();
        eatText = GameObject.FindGameObjectWithTag("Eat");
        thisCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<GUITexture>();
        playerTraits = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTraits>();
        gameManager.SetLevel(1);
        eatTimer = gameObject.AddComponent<Timer>();
        eatTimer.Trigger += EatDisappear;
        eatTimer.Go(eatDisappearTime);
        startTimer = gameObject.AddComponent<Timer>();
        startTimer.Trigger += StartGame;
        startTimer.Go(1f);
        eatText.SetActive(false);
        fade.color = new Color(0, 0, 0, 0);
        PlayerPrefs.DeleteAll();
    }

    void Update() {
        timeRemaining = (int)(levelTime - Time.timeSinceLevelLoad);
        timerText.text = "Time: " + timeRemaining.ToString();
        if (timeRemaining <= 0 && !zooming) {
            zooming = true;
        }
        if (zooming)
            Zoom();
    }

    void StartGame() {
        gameManager.StartLevel();
        eatText.SetActive(true);
        eatTimer.Go(eatDisappearTime);
    }

    void EatAppear() {
        //MAKE THIS APPEAR AFTER A SHORT TIME
    }
    
    void EatDisappear() {
        eatText.SetActive(false);
    }

    void Zoom() {
        timerText.text = "";
        healthText.text = "";
        if (timeRemaining <= 0) {
            timerText.gameObject.SetActive(false);
        }
        zoomSpeed += zoomSpeedAcceleration * Time.deltaTime;
        thisCamera.orthographicSize += zoomSpeed * Time.deltaTime;
        /*if (thisCamera.orthographicSize > 200) {
            Fade();
        }*/
        if (thisCamera.orthographicSize > 250) {
            NextLevel();
        }

    }

    void Fade() {
        fade.color += new Color(0, 0, 0, fadeSpeed);
    }

    void NextLevel() {
        PlayerPrefs.SetInt("Health", playerTraits.Health);
        if (playerTraits.OthersEaten >= playerTraits.PlantsEaten / 4)
            PlayerPrefs.SetInt("Carnivore", 1);
        else
            PlayerPrefs.SetInt("Carnivore", 0);
        Application.LoadLevel("2Live");
    }

}
