using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.DynamoDBv2;
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
    int Bookid = 1001;
    DynamoDBContext Context;
    // Start is called before the first frame update
    void Start()
    {

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

            //AWSの設定
            string region = AWSCognitoIDs.region;
            string IdentityPoolId = AWSCognitoIDs.IdentityPoolId;
            string UserPoolID = AWSCognitoIDs.UserPoolId;
            var _CognitoPoolRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.CognitoPoolRegion);
            Credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoPoolRegion); // ID プールの ID を文字列で指定
            var _DynamoRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.DynamoRegion);
            Client = new AmazonDynamoDBClient(Credentials,RegionEndpoint.USEast1); // ユーザープール使っていない、つまり未認証の権限ロールを使用することになります。
            Context = new DynamoDBContext(Client); // アイテム操作は Context というものを介して操作する模様

            
        }


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
        //PutItem(Client, chat);
        Write();

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
            TableName = "UnityTest",//追加先のテーブル名
                                    //各カラムの値を指定
            Item = new Dictionary<string, AttributeValue>
                {
                    {"ThisIsId",new AttributeValue{S = chatText} },
                }
        };
        //テーブルに追加
        var result = client.PutItemAsync(request).Result;
        

    }
    private void Write()
    {
        //var bookBatch = context.CreateBatchWrite<Book>();

        //Book book1 = new Book
        //{
        //    Test = chatText
        //};

        //bookBatch.AddPutItem(book1);
        //DynamoDBContext context, string chatText


        Book myBook = new Book
        {
            Id = 1001,
            NewValue = "object persistence-AWS SDK for.NET SDK-Book 1001",
            //ISBN = "111-1111111001",
            //BookAuthors = new List<string> { "Author 1", "Author 2" },
        };
        // Save the book.
        CancellationToken result;
        Context.SaveAsync(myBook,result);
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





