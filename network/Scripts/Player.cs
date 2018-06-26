using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public float moveSpeed = 10.0f;		//玩家移动速度
    public float rotateSpeed = 60.0f;   //玩家旋转速度
    public GameObject BustedTank;
    private bool isDead = false;
    public override void OnStartLocalPlayer()
    {
        transform.position = new Vector3(Random.Range(0, 10), 0, -5);
        
    }
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if(isDead)
        {
            return;
        }
        if(GetComponent<Health>().health <= 0)
        {
            ParticleSystem boom = Singleton<Factory>.Instance.GetTankPs();
            boom.transform.position = transform.position;
            boom.Play();
            this.gameObject.SetActive(false);
            GameObject b = Instantiate<GameObject>(BustedTank);
            b.transform.position = transform.position;
            isDead = true;
        }
        if (!isLocalPlayer)
        {
            return;
        }
        Camera.main.transform.position = new Vector3(transform.position.x, 15, transform.position.z);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Cmdfire();
        }
        move();
	}
    public void move()
    {
        float h = Input.GetAxisRaw("Horizontal");	//获取玩家水平轴上的输入
        float v = Input.GetAxisRaw("Vertical"); //获取玩家在垂直方向的输入
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * v);
        //v<0表示获取玩家向后的输入，玩家以moveSpeed的速度向后运动
        transform.Rotate(Vector3.up * h * rotateSpeed * Time.deltaTime);
    }
    [Command]
    public void Cmdfire()
    {
            GameObject b = Singleton<Factory>.Instance.GetBullet();
            b.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z) + transform.forward * 1.5f;
            b.transform.forward = transform.forward;//设置子弹方向
            Rigidbody rb = b.GetComponent<Rigidbody>();
            b.GetComponent<Rigidbody>().velocity = transform.forward * 20;//发射子弹
            NetworkServer.Spawn(b);
    }
}
