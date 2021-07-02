using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using Amazon;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using DG.Tweening;

public class MainMenuUIController : MonoBehaviour
{
    GameObject[] menu_elem;
    GameObject Canvas;

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
    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        GameObject menu_base = GameObject.Find("menu_base");
        menu_elem = new GameObject[menu_base.transform.childCount];
        gameLiftClient = new AmazonGameLiftClient(config.AccessKeyId, config.SecretAccessKey, config.RegionEndPoint);
        for (int i = 0;i < menu_elem.Length;i++)
        {
            menu_elem[i] = menu_base.transform.GetChild(i).gameObject;
            print(menu_elem[i].name);

            EventTrigger eventTrigger = menu_elem[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            switch (i)
            {
                case 0:
                    entry.callback.AddListener((BaseEventData eventData) => {
                        
                    });
                    break;
                case 1:
                    entry.callback.AddListener((BaseEventData eventData) => {
                       
                    });
                    break;
                case 2:
                    entry.callback.AddListener((BaseEventData eventData) => {
                    GameObject obj = (GameObject)Resources.Load("SettingUI");
                    Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);
                    });
                    break;
                case 3:
                    entry.callback.AddListener((BaseEventData eventData) => {
                        Application.Quit();
                        UnityEditor.EditorApplication.isPlaying = false;
                    });
                    break;
                default:
                    break;
            }

            eventTrigger.triggers.Add(entry);
        }
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

    //ルームの検索
    public List<GameSession> SearchRooms()
    {
        UnityEngine.Debug.Log("SearchRooms");
        var response = gameLiftClient.SearchGameSessions(new SearchGameSessionsRequest
        {
            AliasId = config.GameLiftAliasId,
        });
        return response.GameSessions;
    }
}
