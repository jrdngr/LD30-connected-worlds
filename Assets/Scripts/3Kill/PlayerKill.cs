using UnityEngine;
using System.Collections;

public class PlayerKill : MonoBehaviour {

    /*Attack numbers
     * 1. Fire Dash
     * 2. Teleport
     * 3. Booyah
     * 4. Fast Bash
     */
    private int myAttack = 1;
    private bool attacking = false;
    public bool Attacking {
        get { return attacking; }
        set { attacking = value; }
    }
    private PlayerTraits playerTraits;


    void Awake() {
        playerTraits = GetComponent<PlayerTraits>();
    }

    void OnCollisionEnter(Collision other) {
        if (attacking && other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<Death>().Hit(myAttack);
            playerTraits.EnemiesKilled++;
            playerTraits.AddHealth(2);
        }
        if (attacking && other.gameObject.CompareTag("Ally")) {
            other.gameObject.GetComponent<Death>().Hit(myAttack);
            playerTraits.AlliesKilled++;
            playerTraits.AddHealth(1);
        }
        if (attacking && other.gameObject.CompareTag("Bizarro")) {
            other.gameObject.GetComponent<Bizarro>().Hit(25, other.contacts[0].normal * 1000);
        }
    }

    public void SetTraits(bool carnivore, bool killer) {
        if (carnivore && killer)
            myAttack = 3;
        else if (carnivore && !killer)
            myAttack = 4;
        else if (!carnivore && killer)
            myAttack = 1;
        else if (!carnivore && !killer)
            myAttack = 2;
    }

}
