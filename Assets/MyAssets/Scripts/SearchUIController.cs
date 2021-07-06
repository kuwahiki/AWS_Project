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




    }

    // Update is called once per frame
    void Update()
    {
        
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

    

}
