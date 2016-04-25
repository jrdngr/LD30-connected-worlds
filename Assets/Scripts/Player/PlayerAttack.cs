using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

    public float bumpForce;

    private int attackDamage;
    private PlayerTraits playerTraits;
    
    void Awake() {
        playerTraits = GetComponent<PlayerTraits>();
        attackDamage = playerTraits.AttackDamage;
    }

    void OnCollisionEnter(Collision other){
        if (other.gameObject.CompareTag("Other")) {
            other.gameObject.GetComponent<OtherHealth>().Hit(attackDamage, other.contacts[0].normal, bumpForce);
            GetComponent<Rigidbody>().AddForce(other.contacts[0].normal * bumpForce);
        }

    }

}
