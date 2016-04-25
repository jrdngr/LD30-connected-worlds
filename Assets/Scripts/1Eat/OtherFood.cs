using UnityEngine;
using System.Collections;

public class OtherFood : MonoBehaviour {

    private int foodEaten = 0;
    public int FoodEaten {
        get { return foodEaten; }
    }

    public float foodDelayTime;
    private bool foodDelay = false;
    public bool FoodDelay {
        get { return foodDelay; }
    }
    private Timer foodDelayTimer;

    private GameObject eatenEffect;

    void Awake() {
        eatenEffect = (GameObject)Resources.Load("1Eat/FoodEaten");
        foodDelayTimer = gameObject.AddComponent<Timer>();
        foodDelayTimer.Trigger += FoodDelayOver;
    }

    void FoodDelayOver() {
        foodDelay = false;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Food")) {
            foodEaten++;
            foodDelay = true;
            foodDelayTimer.Go(foodDelayTime);
            GameObject eaten = (GameObject)Instantiate(eatenEffect, other.transform.position, Quaternion.identity);
            Destroy(eaten, 3f);
            Destroy(other.gameObject);
        }
    }

}
