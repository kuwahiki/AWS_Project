using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    GameObject Canvas;
    private bool a = true;
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

                GameObject Second = GameObject.Find("SignUpUI(Clone)/Second");
                Second.active = false;
                Destroy(GameObject.Find("LoginUI(Clone)"));

                //ユーザー情報の送信
                Button SignUp_btn = GameObject.Find("Frist/SignUp").GetComponent<Button>();
                SignUp_btn.onClick.AddListener(() =>
                {
                    SignUp();
                    //認証コードを入力するUIへの入れ替え
                    GameObject.Find("SignUpUI(Clone)/Frist").active = false;
                    Second.active = true;

                    GameObject.Find("Submit").GetComponent<Button>().onClick.AddListener(() => {
                        //認証コードの送信
                        Submit();
                        SceneManager.LoadScene("");
                    });
                });
            });
            a = false;
        }
    }

    void SignIn()
    {
        //AWSCognitoのユーザー認証（AWS側の処理）
        //SignInの処理
    }
    
    void SignUp()
    {
        //AWSCognitoのユーザー認証（AWS側の処理）
        //SignUpの処理
    }

    void Submit()
    {
        //AWSCognitoのユーザー認証（AWS側の処理）
        //ConfirmationCodeの送信
    }
}
