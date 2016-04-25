using UnityEngine;
using System.Collections;

public class PlayerFood : MonoBehaviour {

    public int foodHealthValue;
    
    private int foodEaten = 0;
    public int FoodEaten {
        get { return foodEaten; }
    }

    private PlayerTraits playerTraits;
    private GameObject eatenEffect;

    void Awake() {
        eatenEffect = (GameObject)Resources.Load("1Eat/FoodEaten");
        playerTraits = GetComponent<PlayerTraits>();
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Food")) {
            foodEaten++;
            playerTraits.AddHealth(foodHealthValue);
            playerTraits.PlantsEaten++;
            GameObject eaten = (GameObject)Instantiate(eatenEffect, other.transform.position, Quaternion.identity);
            Destroy(eaten, 3f);
            Destroy(other.gameObject);
        }
    }

}
