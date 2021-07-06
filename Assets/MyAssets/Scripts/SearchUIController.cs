using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Aws.GameLift.Realtime.Types;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Internal;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.CognitoIdentity;
using DG.Tweening;

public class SearchUIController : MonoBehaviour
{
    GameObject content, Canvas;
    AmazonGameLiftClient gameLiftClient;
    RealTimeClient realTimeClient;
    CognitoAWSCredentials Credentials;
    AmazonDynamoDBClient Client;
    CanvasGroup group;
    // Start is called before the first frame update
    void Start()
    {
        string region = AWSCognitoIDs.region;
        string IdentityPoolId = AWSCognitoIDs.IdentityPoolId;
        string UserPoolID = AWSCognitoIDs.UserPoolId;
        var _CognitoPoolRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.CognitoPoolRegion);
        var Credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoPoolRegion); // ID プールの ID を文字列で指定
        var _DynamoRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.DynamoRegion);
        Client = new AmazonDynamoDBClient(Credentials, RegionEndpoint.USEast1); // ユーザープール使っていない、つまり未認証の権限ロールを使用することになります。

        gameLiftClient = new AmazonGameLiftClient(GameLiftConfig.AccessKeyId, GameLiftConfig.SecretAccessKey, GameLiftConfig.RegionEndPoint);

        Canvas = GameObject.Find("Canvas");
        content = GameObject.Find("Content");

        var sessions =  SearchRooms();
        foreach (GameSession session in sessions)
        {
            UnityEngine.Debug.Log(session.Name);

            GameObject room_elem = (GameObject)Resources.Load("Room_elem");
            room_elem.name = session.Name;
            Instantiate(room_elem, Canvas.transform.position, Quaternion.identity, content.transform);


            Text RoomName = GameObject.Find(session.Name + "(Clone)/Text").GetComponent<Text>();
            Button JoinRoom = GameObject.Find(session.Name + "(Clone)/Button").GetComponent<Button>();

            RoomName.text = session.Name;
            JoinRoom.onClick.AddListener(() => {
                PlayerPrefs.SetString("TableName", session.Name);
                scanTable();
                try {
                    JoinSession(session.GameSessionId);
                    SceneManager.LoadScene("PlayScene");
                    PlayerPrefs.SetString("TableName",session.Name);

                }
                catch(Exception ex) {
                    UnityEngine.Debug.LogError(ex);
                }               
            });


        }

        group = this.GetComponent<CanvasGroup>();
        group.alpha = 0.01f;
        group.DOFade(1.0f, 1.0f);
        Image back = this.transform.Find("Back").gameObject.GetComponent<Image>();

        EventTrigger trigger = back.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        // なんのイベントを検出するか
        entry.eventID = EventTriggerType.PointerClick;
        // コールバック登録
        entry.callback.AddListener((BaseEventData arg0) => {
            group.DOFade(0.0f,1.0f);
        });
        // EventTriggerに追加
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {
        if(group.alpha <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
    //ルームの検索
    public List<GameSession> SearchRooms()
    {
        UnityEngine.Debug.Log("SearchRooms");
        var response = gameLiftClient.SearchGameSessions(new SearchGameSessionsRequest
        {
            AliasId = GameLiftConfig.GameLiftAliasId,
        });
        return response.GameSessions;
    }

    private void JoinSession(string sessionid)
    {
        var response = gameLiftClient.CreatePlayerSession(new CreatePlayerSessionRequest
        {
            GameSessionId = sessionid,
            PlayerId = SystemInfo.deviceUniqueIdentifier,
        });

        var playerSession = response.PlayerSession;
        ushort DefaultUdpPort = 7777;
        var udpPort = SearchAvailableUdpPort(DefaultUdpPort, DefaultUdpPort + 100);
        realTimeClient = new RealTimeClient(
            playerSession.IpAddress,
            playerSession.Port,
            udpPort,
            ConnectionType.RT_OVER_WS_UDP_UNSECURED,
            playerSession.PlayerSessionId,
            null);

        realTimeClient.OnDataReceivedCallback = OnDataReceivedCallback;
    }

    public void OnDataReceivedCallback(object sender, Aws.GameLift.Realtime.Event.DataReceivedEventArgs e)
    {
    
           UnityEngine.Debug.Log("{e.OpCode}");
    }

    int SearchAvailableUdpPort(int from = 1024, int to = ushort.MaxValue)
    {
        from = Mathf.Clamp(from, 1, ushort.MaxValue);
        to = Mathf.Clamp(to, 1, ushort.MaxValue);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        var set = LsofUdpPorts(from, to);
#else
        var set = GetActiveUdpPorts();
#endif
        for (int port = from; port <= to; port++)
            if (!set.Contains(port))
                return port;
        return -1;
    }

    HashSet<int> LsofUdpPorts(int from, int to)
    {
        var set = new HashSet<int>();
        string command = string.Join(" | ",
            $"lsof -nP -iUDP:{from.ToString()}-{to.ToString()}",
            "sed -E 's/->[0-9.:]+$//g'",
            @"grep -Eo '\d+$'");
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
        });
        if (process != null)
        {
            process.WaitForExit();
            var stream = process.StandardOutput;
            while (!stream.EndOfStream)
                if (int.TryParse(stream.ReadLine(), out int port))
                    set.Add(port);
        }
        return set;
    }

    HashSet<int> GetActiveUdpPorts()
    {
        return new HashSet<int>(IPGlobalProperties.GetIPGlobalProperties()
            .GetActiveUdpListeners().Select(listener => listener.Port));
    }


    // ルームの作成
    void CreateRoom(string roomName = "", int MaxPlyNum = 2)
    {
        UnityEngine.Debug.Log("CreateRoom");
        if (string.IsNullOrEmpty(roomName)) roomName = Guid.NewGuid().ToString();
        var request = new CreateGameSessionRequest
        {
            AliasId = GameLiftConfig.GameLiftAliasId,
            MaximumPlayerSessionCount = MaxPlyNum,
            Name = roomName
        };
        var response = gameLiftClient.CreateGameSession(request);
    }

    private void CreateTable(IAmazonDynamoDB client, string tableName)
    {
        //リクエストを構築
        var request = new CreateTableRequest
        {
            //テーブルの列情報を設定
            //「ThisIsId」と「ThisIsSomthing」という2つの列を持つテーブルを作る
            AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",//カラム名
                        AttributeType = "N"
                    },
                    //new AttributeDefinition
                    //{
                    //    AttributeName = "Date",//カラム名
                    //    AttributeType = "N"
                    //},
                    //new AttributeDefinition
                    //{
                    //    AttributeName = "Text",//カラム名
                    //    AttributeType = "S"
                    //},
                    //new AttributeDefinition
                    //{
                    //    AttributeName = "UserName",//カラム名
                    //    AttributeType = "S"
                    //}
                },
            //勉強中
            KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH" //Partition key
                    }
                },
            //勉強中
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            },
            //テーブル名
            TableName = tableName
        };

        var result = client.CreateTableAsync(request).Result;
    }

    private async void scanTable() {
        try
        {
            string TableName = PlayerPrefs.GetString("TableName");
            var req = new ScanRequest(TableName);
            var res = await Client.ScanAsync(req);
        }
        catch (Exception ex)
        {
            CreateTable(Client,PlayerPrefs.GetString("TableName"));
        }
    }
    
}
