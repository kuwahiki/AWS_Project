using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Amazon;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Aws.GameLift.Realtime.Types;
using Aws.GameLift.Realtime;

public class charController : MonoBehaviour
{
    [SerializeField] private Transform MainCamera;
    [SerializeField] private float MaxSpeed = 3.0f;

    private const int foword = 1, back = 2,right = 3,left = 4;
    private int idel, walk;
    private string sex;
    private Animator animator;
    Rigidbody rigidbody;

    class GameLiftConfig
    {
        public RegionEndpoint RegionEndPoint { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string GameLiftAliasId { get; set; }
    }

   GameLiftConfig config = new GameLiftConfig
    {
        RegionEndPoint = RegionEndpoint.USEast1,
        AccessKeyId = "AKIAX5IKZ7C57E7JG2XI",
        SecretAccessKey = "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",
        GameLiftAliasId = "alias-e8833599-f4ae-4d6d-98db-37dbed92b017"
    };
    AmazonGameLiftClient gameLiftClient;
    RealTimeClient realTimeClient;
    int playrsenderID;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.Find("Main Camera").transform;
        rigidbody = this.GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();

        idel = animator.GetInteger("idel");
        walk = animator.GetInteger("walk");
        PlayerPrefs.SetInt("State", 0);
        sex = PlayerPrefs.GetString("sex");

        gameLiftClient = new AmazonGameLiftClient(config.AccessKeyId,config.SecretAccessKey,config.RegionEndPoint);
        realTimeClient = GameObject.Find("RealTimeClient").GetComponent<Configobjcontroller>().realTimeClient;
        playrsenderID = PlayerPrefs.GetInt("PlayerSendID");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        int Chat = PlayerPrefs.GetInt("State");
        int animationInt = idel;
        Vector3 foword = this.transform.position - MainCamera.position;
        foword.y = 0;
        foword = foword.normalized * 10;
        if (Chat == 0)
        {
            if (Input.GetKey(KeyCode.W))
            {
                //??????????????????????????????
                rigidbody.AddForce(foword);
                animationInt = walk;
                realTimeClient.SendMessage(DeliveryIntent.Reliable, "test",PlayerMoveMes.MoveFront);
                
            }
            if (Input.GetKey(KeyCode.A))
            {
                //???????????????????????????
                Vector3 left = new Vector3(-foword.z, 0, foword.x);
                rigidbody.AddForce(left);
                animationInt = walk;
                realTimeClient.SendMessage(DeliveryIntent.Reliable, "test", PlayerMoveMes.MoveLeft);
            }
            if (Input.GetKey(KeyCode.D))
            {
                //???????????????????????????
                Vector3 right = new Vector3(foword.z, 0, -foword.x);
                rigidbody.AddForce(right);
                animationInt = walk;
                realTimeClient.SendMessage(DeliveryIntent.Reliable, "test", PlayerMoveMes.MoveRight);
            }
            if (Input.GetKey(KeyCode.S))
            {
                //???????????????????????????
                Vector3 back = -foword;
                rigidbody.AddForce(back);
                animationInt = walk;
                realTimeClient.SendMessage(DeliveryIntent.Reliable, "test", PlayerMoveMes.MoveBack);
            }
        }
        animator.SetInteger("animation",animationInt);

        if (animationInt == 1)
        {
            //???????????????????????????????????????
            Vector3 force = rigidbody.velocity * 100.0f;
            force += this.transform.position;
            force.y = 0;

            if (force != this.transform.position)
            {
                this.transform.rotation = Quaternion.LookRotation(force);
            }
        }

        if (this.sex != PlayerPrefs.GetString("sex"))
        {
            //?????????????????????????????????????????????
            UnityEngine.Debug.Log(this.transform.root.name);
            GameObject Model = GameObject.Find(this.transform.root.name + "/Model");
            if (Model == null)
            {
                UnityEngine.Debug.Log("null");
                Model = GameObject.Find(this.transform.root.name + this.sex + "(Clone)");
            }
            targetController target = GameObject.Find(this.transform.root.name + "/CameraTarget").GetComponent<targetController>();
            GameObject NewModel = (GameObject)Resources.Load(PlayerPrefs.GetString("sex"));
            Instantiate(NewModel, this.gameObject.transform.position, this.transform.rotation, this.transform.root.gameObject.transform);
            target.PlayerModel = GameObject.Find(this.transform.root.name + PlayerPrefs.GetString("sex") + "(Clone)");
            Destroy(Model);
            this.sex = PlayerPrefs.GetString("sex");
        }
    }

    private void LookDirection(int direction)
    {
        switch (direction)
        {
            case foword:
                break;

        }
    }

    public void OnDataReceivedCallback(object sender, Aws.GameLift.Realtime.Event.DataReceivedEventArgs e)
    {
        
            UnityEngine.Debug.Log( $"{e.OpCode}");
    }
}

