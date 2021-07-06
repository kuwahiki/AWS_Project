using System.Collections;
using System.IO;
using System.Linq;
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
    float getchat = 0;
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
        PutItem(Client, chat,PlayerPrefs.GetString("TableName"));
        //CreateTable(Client, "test");

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

    private void PutItem(IAmazonDynamoDB client, string chatText,string Table)
    {
        getItemChat();
        //リクエストの構築
        count++;
        DateTime dt = DateTime.Now;
        string nowtime = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + dt.Millisecond.ToString();
        PutItemRequest request = new PutItemRequest
        {
            TableName = Table,//追加先のテーブル名
                                    //各カラムの値を指定
            Item = new Dictionary<string, AttributeValue>
                {
                    {"Id",new AttributeValue{N = count.ToString()} },
                    {"Date",new AttributeValue{N = nowtime} },
                    {"Text",new AttributeValue{S = chatText } },
                    {"UserName",new AttributeValue{S = "TestUser" } }
                }
        };
        //テーブルに追加
        var result = client.PutItemAsync(request).Result;
        getItemChat();

    }

    private void Update()
    {
        if(getchat >= 10)
        {           
           getItemChat();
            getchat = 0;
        }
        else {
            getchat += Time.deltaTime;
        }


    }

    private async Task getItemChat()
    {
        //GetItemRequest request = new GetItemRequest
        //{
        //    Key = new Dictionary<string, AttributeValue>()
        //    {
        //        { "UserName", new AttributeValue {S = "TestUser"} }
        //    },
        //    TableName = "UnityChat",                            
        //};

        try {
            //var res1 = await Client.GetItemAsync(request);
            string TableName = PlayerPrefs.GetString("TableName");
            var req = new ScanRequest(TableName);
            var res = await Client.ScanAsync(req);
            Text  text= GameObject.Find("Canvas/Panel/Text").GetComponent<Text>();
            var items = res.Items;
            int i = 0;
            text.text = null;
            var chatdate = new Dictionary<string, string>();
            foreach(IDictionary<string, AttributeValue> item in items)
            {
                setChat(item,chatdate);
                i++;
                
            }
            int set =  1;
            Debug.Log(i);
            if(i >= 14)
            {
                set = i - 12;
            }
            for(int j = set; j  <= i; j++)
            {
                Debug.Log(j);
                text.text +=  chatdate[j.ToString()];
            }
            count = i;
            Debug.Log(count+ "\n" + set);
            //PrintChat(text,res.Items); 
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }

    }


    private void setChat(IDictionary<string, AttributeValue> attributeList, Dictionary<string, string> data)
    {
        string Username = "", chat = "", ID = "";
        foreach (KeyValuePair<string,AttributeValue> kvp in attributeList)
        {
            string attributeName = kvp.Key;
            Debug.Log(kvp.Key);
            AttributeValue Value = kvp.Value;

            switch(attributeName){
                case "Id":
                    ID = Value.N;
                    break;
                case "Text":
                    chat = Value.S;
                    break;
                case "UserName":
                    Username = Value.S;
                    break;
                default:
                    break;
            }
            if(chat != "" && Username != "")
            {
                string str = Username + ":" + chat + "\n";
                chat = "";
                Username = chat;
                Debug.Log(ID + ":" + str);
                data.Add(ID, str);
            }
        }
        //var setchat = chatdate.OrderBy(x => x.Key);
        //foreach(var res in chatdate) {
        //    Debug.Log("id ="  + res.Key);
        //    text.text += res.Value;
        //}
        
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





