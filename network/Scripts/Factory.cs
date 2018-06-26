using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour {
    public GameObject bullet;
    public ParticleSystem bulletPs;
    public ParticleSystem tankPs;
    // Use this for initialization
    private Queue<GameObject> bullets = new Queue<GameObject>();
    private List<ParticleSystem> bulletPses = new List<ParticleSystem>();
    private List<ParticleSystem> tankPses = new List<ParticleSystem>();
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
	}
    public GameObject GetBullet()
    {
        GameObject b = null;
        Debug.Log(bullets.Count);
        if (bullets.Count == 0)
        {
            b = Instantiate<GameObject>(bullet);
            return b;
        }
        b = bullets.Dequeue();
        return b;
    }

    public void RecycleBullet(GameObject b)
    {
        b.SetActive(false);
        bullets.Enqueue(b);
    }

    public ParticleSystem GetBulletPs()
    {
        for (int i = 0; i < bulletPses.Count; ++i)
        {
            if (!bulletPses[i].isPlaying)
            {
                return bulletPses[i];
            }
        }
        ParticleSystem p = Instantiate<ParticleSystem>(bulletPs);
        bulletPses.Add(p);
        return p;
    }
    public ParticleSystem GetTankPs()
    {
        for (int i = 0; i < tankPses.Count; ++i)
        {
            if (!tankPses[i].isPlaying)
            {
                return tankPses[i];
            }
        }
        ParticleSystem p = Instantiate<ParticleSystem>(tankPs);
        tankPses.Add(p);
        return p;
    }
}
