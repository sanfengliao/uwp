using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    // Use this for initialization
   
	void Start () {
       
	}
	
	// Update is called once per frame
	
    public const int maxHealth = 100;
    [SyncVar (hook = "onHealthChange")]
    public int health = maxHealth;

    public void TakeDamage(int amount)
    {
        
        if(!isServer)
        {
            return;
        }
       
        health -= amount;
       
    }

    void onHealthChange(int health)
    {

        //Debug.Log("=====");
        this.health = health;
        if(isLocalPlayer)
        {
          
            GameObject.FindWithTag("PlayerHealth").GetComponent<Slider>().value = health;
        }
        else
        {
            GameObject.FindWithTag("EnemyHealth").GetComponent<Slider>().value = health;
        }
    }
}
