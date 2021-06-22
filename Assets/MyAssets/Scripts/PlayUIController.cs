using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayUIController : MonoBehaviour
{
    [SerializeField] private Image[] Icons = new Image[2];
    private Button Send;
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
                SendChat();
                GameObject parent = GameObject.Find("InputChat(Clone)");
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

    void SendChat()
    {
        //チャット内容をDynamoDBに送信する処理（AWS側の処理）

    }
}
