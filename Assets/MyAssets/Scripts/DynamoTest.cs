using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amazon;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Internal;
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

    private void RetrieveBook()
    {
        Debug.Log ("\n*** Load book**\n");
        // Retrieve the book.
        Book bookRetrieved = null;
        int bookID = bookRetrieved.Id;

        //Context.LoadAsync<Book>(1,(AmazonDynamoResult<Book> result) =>{
        //    if (result.Exception != null)
        //    {

        //       Debug.Log("LoadAsync error" + result.Exception.Message);
        //       Debug.LogException(result.Exception);
        //       return;
        //    }
        //var _retrievedBook = result.Response;
        //Debug.Log("Retrieved Book: " + "\nId=" + _retrievedBook.Id + "\nTitle=" + _retrievedBook.Title + "\nISBN=" + _retrievedBook.ISBN);
        //string authors = "";
        //foreach (string author in _retrievedBook.BookAuthors)
        //     authors += author + ",";
        //     this.displayMessage += "\nBookAuthor= " + authors;
        //     this.displayMessage += ("\nDimensions= " + _retrievedBook.Dimensions.Length + " X " + _retrievedBook.Dimensions.Height + " X " +  _retrievedBook.Dimensions.Thickness);

        //}, null);
    }

}


