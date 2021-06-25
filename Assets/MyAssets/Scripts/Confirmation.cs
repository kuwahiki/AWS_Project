using System;
using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

public class Confirmation : MonoBehaviour
{
    public InputField emailField;
    public InputField confirmationCodeField;
    static string appClientId = AWSCognitoIDs.AppClientId;

    CognitoAWSCredentials credentials = new CognitoAWSCredentials(
    "us-east-1:15fce7cd-2499-4100-8bae-4c31a9cf03b8", // ID プールの ID
    RegionEndpoint.USEast1 // リージョン
    );
    public void OnClick()
    {
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