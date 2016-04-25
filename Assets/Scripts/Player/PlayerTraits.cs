using UnityEngine;
using System.Collections;

public class PlayerTraits : MonoBehaviour {

    //DEBUGGING SUPER POWER POWERS
    public bool eatsBunnies;
    public bool disturbed;
    public bool charmingSmile;

    //Eat!
    private int health = 0;
    public int Health {
        get { return health; }
    }
    private int attackDamage = 10;
    public int AttackDamage {
        get { return attackDamage; }
        set { attackDamage = value; }
    }
    private int plantsEaten = 0;
    public int PlantsEaten {
        get { return plantsEaten; }
        set { plantsEaten = value; }
    }
    private int othersEaten = 0;
    public int OthersEaten {
        get { return othersEaten; }
        set { othersEaten = value; }
    }
    private bool carnivore;
    public bool Carnivore {
        get { return carnivore; }
        set { carnivore = value; }
    }
    public float redTime;
    
    //Live!
    private bool killer;
    public bool Killer {
        get { return killer; }
        set { killer = value; }
    }
    private int oldHitpoints;
    public int OldHitpoints {
        get { return oldHitpoints; }
        set { oldHitpoints = value; }
    }

    //Kill!
    private int alliesKilled = 0;
    public int AlliesKilled {
        get { return alliesKilled; }
        set { alliesKilled = value; }
    }
    private int enemiesKilled = 0;
    public int EnemiesKilled {
        get { return enemiesKilled; }
        set { enemiesKilled = value; }
    }
    
    //Win!
    private bool hasAllies;
    public bool HasAllies {
        get { return hasAllies; }
        set { hasAllies = value; }
    }

    private bool isDead = false;
    public bool IsDead {
        get { return isDead; }
    }

    private Timer redTimer;
    private GameManager gameManager;
    private PlayerMovement playerMovement;
    private GameObject player;
    private GameObject bizarro;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        redTimer = gameObject.AddComponent<Timer>();
        redTimer.Trigger += RedOff;
    }

    void Start() {
        switch (gameManager.CurrentLevel) {
            case 1:
                break;
            case 2:
                health = PlayerPrefs.GetInt("Health");
                carnivore = (PlayerPrefs.GetInt("Carnivore") == 1) ? true : false;
                break;
            case 3:
                health = PlayerPrefs.GetInt("Health");
                carnivore = (PlayerPrefs.GetInt("Carnivore") == 1) ? true : false;
                killer = (PlayerPrefs.GetInt("Killer") == 1) ? true : false;
                break;
            case 4:
                health = PlayerPrefs.GetInt("PlayerHealth");
                oldHitpoints = PlayerPrefs.GetInt("Health");
                carnivore = (PlayerPrefs.GetInt("Carnivore") == 1) ? true : false;
                killer = (PlayerPrefs.GetInt("Killer") == 1) ? true : false;
                hasAllies = (PlayerPrefs.GetInt("HasAllies") == 1) ? true : false;
                break;
            default:
                break;
        }
        /*
        //DEBUGGING
        carnivore = eatsBunnies;
        killer = disturbed;
        hasAllies = charmingSmile;
        //DEBUGGING

        health = 250;
        */
        if (gameManager.CurrentLevel > 1)
            playerMovement.SetPowers();
        if (gameManager.CurrentLevel == 4) {
            bizarro = GameObject.FindGameObjectWithTag("Bizarro");
            gameManager.GetComponent<WinLevel>().Choices();
            bizarro.GetComponent<Bizarro>().SetTraits(carnivore, killer, hasAllies);
            bizarro.GetComponent<BizarroAttack>().SetTraits(carnivore, killer, hasAllies);
            player.GetComponent<PlayerWin>().SetTraits(carnivore, killer, hasAllies);
        }
    }

    void Update() {
        if (health <= 0 && gameManager.CurrentLevel == 2)
            gameManager.GetComponent<LiveLevel>().YouDied();
        if (health <= 0 && gameManager.CurrentLevel == 4)
            gameManager.GetComponent<WinLevel>().YouDied();

    }

    public void AddHealth(int amount) {
        health += amount;
    }

    public void Hit(int damage) {
        health -= damage;
        GetComponent<Renderer>().material.color = Color.red;
        redTimer.Go(redTime);
    }

    public void Kill() {
        isDead = true;
        if (gameManager.CurrentLevel == 2)
            gameManager.GetComponent<LiveLevel>().YouDied();
    }

    public void RedOff() {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
