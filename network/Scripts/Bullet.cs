using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int type; // 0 表示player的子弹， 1 表示enemy的子弹
                     // Use this for initialization
    public int enemyHealth;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision collision)
    {
        
       
        if (collision.gameObject.tag == "Player")
        {
           collision.gameObject.GetComponent<Health>().TakeDamage(10);
        }

        ParticleSystem boom = Singleton<Factory>.Instance.GetBulletPs();
        boom.transform.position = transform.position;
        boom.Play();
        if (this.gameObject.activeSelf)
        {
            Singleton<Factory>.Instance.RecycleBullet(this.gameObject);
        }
    }
   
}
