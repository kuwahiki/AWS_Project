using System;
using UnityEngine;
using UnityEngine.UI;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider; // for AmazonCognitoIdentityProviderClient
using Amazon.Extensions.CognitoAuthentication; // for CognitoUserPool
using Amazon; // for RegionEndpoint

public class Signin : MonoBehaviour
{
    public InputField emailField;
    public InputField passwordField;
    public Text resultText;
    static string appClientId = AWSCognitoIDs.AppClientId;
    static string userPoolId = AWSCognitoIDs.UserPoolId;

    CognitoAWSCredentials credentials = new CognitoAWSCredentials(
    "us-east-1:15fce7cd-2499-4100-8bae-4c31a9cf03b8", // ID プールの ID
    RegionEndpoint.USEast1 // リージョン
    );

    public void OnClick()
    {
        try
        {
            AuthenticateWithSrpAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

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
        resultText.text = user.SessionTokens.IdToken;
    }
}