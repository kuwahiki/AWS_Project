using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.GameLift;
using Amazon.GameLift.Model;

public class CreateUIController : MonoBehaviour
{
    AmazonGameLiftClient gameLiftClient;
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
        AccessKeyId = "AKIAX5IKZ7C5WR7YBJ5W",
        SecretAccessKey = "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",
        GameLiftAliasId = "alias-69cf940b-7d2a-4cad-a2c0-6e21792217f5"
    };
    // Start is called before the first frame update
    void Start()
    {
        gameLiftClient = new AmazonGameLiftClient(config.AccessKeyId, config.SecretAccessKey, config.RegionEndPoint);
        Button Create_Btn = GameObject.Find("Create_Room").GetComponent<Button>();
        Create_Btn.onClick.AddListener(() => {
            GameObject Room_name_obj, Room_Num_obj;
            Room_name_obj = GameObject.Find("Input_Name");
            Room_Num_obj = GameObject.Find("Select_Num/Label");

            string name = Room_name_obj.GetComponent<InputField>().text;
            int Max_num = Int32.Parse(Room_Num_obj.GetComponent<Text>().text);

            CreateRoom();
            Destroy(this.gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ルームの作成
    void CreateRoom(string roomName = "")
    {
        UnityEngine.Debug.Log("CreateRoom");
        if (string.IsNullOrEmpty(roomName)) roomName = Guid.NewGuid().ToString();
        var request = new CreateGameSessionRequest
        {
            AliasId = config.GameLiftAliasId,
            MaximumPlayerSessionCount = 2,
            Name = roomName
        };
        var response = gameLiftClient.CreateGameSession(request);
    }
}
