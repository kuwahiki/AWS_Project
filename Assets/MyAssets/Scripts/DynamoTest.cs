using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;

public class DynamoTest : MonoBehaviour
{
    AmazonDynamoDBClient Client;
    int Bookid = 1001;
    DynamoDBContext Context;
    // Start is called before the first frame update
    void Start()
    {
        //AWSの設定
        //UnityInitializer.AttachToGameObject(this.gameObject);
        //AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest; // ←この行で UnityWebRequest 使うように指定
        string region = AWSCognitoIDs.region;
        string IdentityPoolId = AWSCognitoIDs.IdentityPoolId;
        string UserPoolID = AWSCognitoIDs.UserPoolId;
        var _CognitoPoolRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.CognitoPoolRegion);
        var Credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoPoolRegion); // ID プールの ID を文字列で指定
        var _DynamoRegion = RegionEndpoint.GetBySystemName(AWSCognitoIDs.DynamoRegion);
        Credentials.AddLogin("cognito-idp." + region + ".amazonaws.com/" + UserPoolID, AWSCognitoIDs.IDToken);
        Client = new AmazonDynamoDBClient(Credentials, _DynamoRegion); // ユーザープール使っていない、つまり未認証の権限ロールを使用することになります。
        Context = new DynamoDBContext(Client); // アイテム操作は Context というものを介して操作する模様
    }

    private void PerformUpdateOperation()
    {
        // Retrieve the book. 
        //Book bookRetrieved = null;
        //Context.LoadAsync<Book>(Bookid, (result) => // Update なので、値が取れることは期待
        //{
        //    if (result.Exception == null)
        //    {
        //        bookRetrieved = result.Result as Book; // ここは期待通りなら強制キャスト
        //                                               // Update few properties.
        //        bookRetrieved.ISBN = "222-2222221001";
        //        bookRetrieved.BookAuthors = new List<string> { " Author 1", "Author x" }; // Replace existing authors list with this.
        //        Context.SaveAsync<Book>(bookRetrieved, (res) => // 更新したデータ構造をそのままコピーします
        //        {
        //            if (res.Exception == null)
        //                resultText.text += ("\nBook updated");
        //        });
        //    }
        //});
    }
}

