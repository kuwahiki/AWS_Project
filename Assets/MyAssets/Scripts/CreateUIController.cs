using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Internal;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.CognitoIdentity;
using DG.Tweening;

public class CreateUIController : MonoBehaviour
{
    AmazonGameLiftClient gameLiftClient;
    CognitoAWSCredentials Credentials;
    AmazonDynamoDBClient Client;
    // Start is called before the first frame update
    void Start()
    {
        string region = AWSCognitoIDs.region;
        string IdentityPoolId = AWSCognitoIDs.IdentityPoolId;
        string UserPoolID = AWSCognitoIDs.UserPoolId;
        var _CognitoPoolRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.CognitoPoolRegion);
        Credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoPoolRegion); // ID プールの ID を文字列で指定
        var _DynamoRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.DynamoRegion);
        Client = new AmazonDynamoDBClient(Credentials, RegionEndpoint.USEast1); // ユーザープール使っていない、つまり未認証の権限ロールを使用することになります。

        gameLiftClient = new AmazonGameLiftClient(GameLiftConfig.AccessKeyId, GameLiftConfig.SecretAccessKey, GameLiftConfig.RegionEndPoint);
        Button Create_Btn = GameObject.Find("Create_Room").GetComponent<Button>();
        Create_Btn.onClick.AddListener(() => {
            GameObject Room_name_obj, Room_Num_obj;
            Room_name_obj = GameObject.Find("Input_Name");
            Room_Num_obj = GameObject.Find("Select_Num/Label");

            string name = Room_name_obj.GetComponent<InputField>().text;
            int Max_num = Int32.Parse(Room_Num_obj.GetComponent<Text>().text);

            Debug.Log(name + "\n" + Max_num);
            if (name != "")
            {
                 CreateRoom(name,Max_num);
                CreateTable(Client, name);

            }


            Destroy(this.gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ルームの作成
    void CreateRoom(string roomName = "",int MaxPlyNum = 2)
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
}
