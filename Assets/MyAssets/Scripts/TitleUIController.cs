using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System;

public class TitleUIController : MonoBehaviour
{
    GameObject Canvas,Second;
    private bool a = true;
    private InputField emailField;
    private InputField passwordField;
    private Text resultText;
    public InputField confirmationCodeField;
    static string appClientId = AWSCognitoIDs.AppClientId;
    static string userPoolId = AWSCognitoIDs.UserPoolId;
    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && a == true)
        {
            GameObject obj = (GameObject)Resources.Load("LoginUI");
            Button button;
            //SignIn用のUIの作成
            Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);

            //SignInボタンの処理
            button = GameObject.Find("LoginUI(Clone)/SignIn").GetComponent<Button>();
            button.onClick.AddListener(()=> {
                SignIn();
            });

            //SignUpボタンの処理
            button = GameObject.Find("LoginUI(Clone)/SignUp").GetComponent<Button>();
            button.onClick.AddListener(() =>
            {

                //SignUp用のUIを作成
                obj = (GameObject)Resources.Load("SignUpUI");
                Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);

                Second = GameObject.Find("SignUpUI(Clone)/Second");
                Second.active = false;
                Destroy(GameObject.Find("LoginUI(Clone)"));

                //ユーザー情報の送信
                Button SignUp_btn = GameObject.Find("Frist/SignUp").GetComponent<Button>();
                SignUp_btn.onClick.AddListener(() =>
                {
                    SignUp();
                });
            });
            a = false;
        }
    }



    void SignIn() {
        //AWSCognitoのユーザー認証（AWS側の処理）
        //SignInの処理
        emailField = GameObject.Find("Email_Input").GetComponent<InputField>();
        passwordField = GameObject.Find("Pass_Input").GetComponent<InputField>();
        try
        {
            AuthenticateWithSrpAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        "us-east-1:15fce7cd-2499-4100-8bae-4c31a9cf03b8", // ID プールの ID
        RegionEndpoint.USEast1 // リージョン
        );

    public async void AuthenticateWithSrpAsync()
    {

        var provider = new AmazonCognitoIdentityProviderClient(credentials, RegionEndpoint.USEast1);
        CognitoUserPool userPool = new CognitoUserPool(
            userPoolId,
            appClientId,
            provider
        );
        CognitoUser user = new CognitoUser(
            emailField.text,
            appClientId,
            userPool,
            provider
        );

        AuthFlowResponse context = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
        {
            Password = passwordField.text
        }).ConfigureAwait(true);

        // for debug
        Debug.Log("success");
        SceneManager.LoadScene("PlayScene");
    }

    void SignUp()
    {
        //AWSCognitoのユーザー認証（AWS側の処理）
        //SignUpの処理
        emailField = GameObject.Find("Email_Input").GetComponent<InputField>();
        passwordField = GameObject.Find("Pass_Input").GetComponent<InputField>();


        var client = new AmazonCognitoIdentityProviderClient(null, Amazon.RegionEndpoint.USEast1);
        var sr = new SignUpRequest();
        string email = emailField.text;
        string password = passwordField.text;

        sr.ClientId = appClientId;
        sr.Username = email;
        sr.Password = password;
        sr.UserAttributes = new List<AttributeType> {
            new AttributeType {
                Name = "email",
                Value = email
            }
        };

        try
        {
            SignUpResponse result = client.SignUp(sr);
            Debug.Log(result);
            //認証コードを入力するUIへの入れ替え
            GameObject.Find("SignUpUI(Clone)/Frist").active = false;
            Second.active = true;

            GameObject.Find("Submit").GetComponent<Button>().onClick.AddListener(() => {
                //認証コードの送信
                Submit();

            });
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    void Submit()
    {
        //AWSCognitoのユーザー認証（AWS側の処理）
        //ConfirmationCodeの送信

        confirmationCodeField = GameObject.Find("ConfirmationCode_Input").GetComponent<InputField>();
        var client = new AmazonCognitoIdentityProviderClient(credentials, Amazon.RegionEndpoint.USEast1);
        ConfirmSignUpRequest confirmSignUpRequest = new ConfirmSignUpRequest();

        confirmSignUpRequest.Username = emailField.text;
        confirmSignUpRequest.ConfirmationCode = confirmationCodeField.text;
        confirmSignUpRequest.ClientId = appClientId;

        try
        {
            ConfirmSignUpResponse confirmSignUpResult = client.ConfirmSignUp(confirmSignUpRequest);
            Debug.Log(confirmSignUpResult.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
