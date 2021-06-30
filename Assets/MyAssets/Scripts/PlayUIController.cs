using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Internal;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.CognitoIdentity;
using System.Threading.Tasks;
using System.Threading;

public class PlayUIController : MonoBehaviour
{
    [SerializeField] private Image[] Icons = new Image[2];
    private Button Send;
    AmazonDynamoDBClient Client;
    CognitoAWSCredentials Credentials;
    int Bookid = 1001,count = 0;
    DynamoDBContext Context;
    bool getchat = false;
    // Start is called before the first frame update
    void Start()
    {
        //AWSの設定
        string region = AWSCognitoIDs.region;
        string IdentityPoolId = AWSCognitoIDs.IdentityPoolId;
        string UserPoolID = AWSCognitoIDs.UserPoolId;
        var _CognitoPoolRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.CognitoPoolRegion);
        Credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoPoolRegion); // ID プールの ID を文字列で指定
        var _DynamoRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.DynamoRegion);
        Client = new AmazonDynamoDBClient(Credentials,RegionEndpoint.USEast1); // ユーザープール使っていない、つまり未認証の権限ロールを使用することになります。
        Context = new DynamoDBContext(Client); // アイテム操作は Context というものを介して操作する模様

        for (int i = 0; i < Icons.Length; i++)
        {
            EventTrigger trigger = Icons[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();

            // なんのイベントを検出するか
            entry.eventID = EventTriggerType.PointerClick;
            // コールバック登録
            if (i == 0)
            {
                entry.callback.AddListener(Clicked_Chat);
            }
            if(i == 1)
            {
                entry.callback.AddListener(Clicked_Menu);
            }

            // EventTriggerに追加
            trigger.triggers.Add(entry);
            //Debug.Log();

            

           
        }
        getItemChat();
    }


    void Clicked_Chat(BaseEventData eventData)
    {
            //チャット入力用のUIの生成
            GameObject obj = (GameObject)Resources.Load("InputChat");
            Transform Canvas = GameObject.Find("Canvas").transform;
            Instantiate(obj,this.transform.position, Quaternion.identity, Canvas);

            //プレイヤーが動けないようにする
            PlayerPrefs.SetInt("State", 1);
            obj = GameObject.Find("InputChat(Clone)/Send");

            //チャット内容をDynamoDBに送信する処理（Unity側の処理）
            Send = obj.GetComponent<Button>();
            Send.onClick.AddListener(() => {
                GameObject parent = GameObject.Find("InputChat(Clone)");
                string chat = parent.transform.Find("InputField").GetComponent<InputField>().text;
                SendChat(chat);
                Destroy(parent);
                //プレイヤーが動けるようにする
                PlayerPrefs.SetInt("State", 0);
            });
    }

    void Clicked_Menu(BaseEventData eventData)
    {
        GameObject obj = (GameObject)Resources.Load("PlayMenu");
        Transform Canvas = GameObject.Find("Canvas").transform;
        Instantiate(obj,this.transform.position, Quaternion.identity, Canvas);
        PlayerPrefs.SetInt("State", 1);
    }

    void SendChat(string chat)
    {
        //チャット内容をDynamoDBに送信する処理（AWS側の処理）
        PutItem(Client, chat);
        //CreateTable(Client, "test");
        count++;

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
                        AttributeName = "ThisIsId",//カラム名
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "ThisIsSomething",//カラム名
                        AttributeType = "N"
                    }
                },
            //勉強中
            KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "ThisIsId",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "ThisIsSomething",
                        KeyType = "RANGE" //Sort key
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

    private void PutItem(IAmazonDynamoDB client, string chatText)
    {
        //リクエストの構築
        PutItemRequest request = new PutItemRequest
        {
            TableName = "UnityChat",//追加先のテーブル名
                                    //各カラムの値を指定
            Item = new Dictionary<string, AttributeValue>
                {
                    {"Name",new AttributeValue{S = "TestUser" + count.ToString()} },
                    {"Chat",new AttributeValue{S = chatText } }
                }
        };
        //テーブルに追加
        var result = client.PutItemAsync(request).Result;
        

    }

    private void Update()
    {
        if(getchat == false)
        {
           //getItemChat();
        }

    }

    private async Task getItemChat()
    {
        GetItemRequest request = new GetItemRequest
        {
            Key = new Dictionary<string, AttributeValue>() { { "Name", new AttributeValue {S = "TestUser"} } },
            TableName = "UnityChat"
        };

        if(request == null)
        {
            Debug.Log("null");
        }
        try {
            //var res = await Client.GetItemAsync(request);
            var req = new ScanRequest("UnityChat");
            var res = await Client.ScanAsync(req);
            Text  text= GameObject.Find("Canvas/Panel/Text").GetComponent<Text>();
            var items = res.Items;
            //PrintChat(text,res.Items); 
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
        Debug.Log("test");

    }

    private void PrintChat(Text ChatCanvas,IDictionary<string, AttributeValue> attributeList)
    {
        Debug.Log("chatprint");
        int i = 0;
        string Username = "", chat = "";
        foreach (KeyValuePair<string,AttributeValue> kvp in attributeList)
        {
            string attributeName = kvp.Key;
            AttributeValue Value = kvp.Value;

            //Debug.Log(attributeName +"," +
            //    (Value.S == null ? "" : "S=[" + Value.S + "]") +
            //        (Value.N == null ? "" : "N=[" + Value.N + "]") +
            //        (Value.SS == null ? "" : "SS=[" + string.Join(",", Value.SS.ToArray()) + "]") +
            //        (Value.NS == null ? "" : "NS=[" + string.Join(",", Value.NS.ToArray()) + "]")
            //    );
            if(i%2 == 0)
            {
                chat = Value.S;
            }
            else
            {
                Username = Value.S;
                ChatCanvas.text = Username + ":" + chat + "\n";
            }
            i++;
        }

        Debug.Log(ChatCanvas.text);

    }
}

[DynamoDBTable("BookStore"),Serializable]
public class Book
{
    [DynamoDBHashKey]   // Hash key.
    public int Id { get; set; }
    [DynamoDBProperty]
    public string NewValue { get; set; }
    //[DynamoDBProperty]
    //public string ISBN { get; set; }
    //[DynamoDBProperty("Authors")]    // Multi-valued (set type) attribute.
    //public List<string> BookAuthors { get; set; }
}





