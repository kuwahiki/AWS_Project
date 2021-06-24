using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LogoutUIController : MonoBehaviour
{
    GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject;
        Button title = parent.transform.Find("Back_title").GetComponent<Button>();
        title.onClick.AddListener(() => {
            SceneManager.LoadScene("Title");
            Logout();
        });
        Button Main = parent.transform.Find("Back_Menu").GetComponent<Button>();
        Main.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void Logout()
    {
        //ゲームからのログアウト（AWS側の処理）
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
