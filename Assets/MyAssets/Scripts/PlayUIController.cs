using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayUIController : MonoBehaviour
{
    [SerializeField] private Image[] Icons = new Image[2];
    string IconName;
    private Button Send;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Icons.Length; i++)
        {
            IconName = Icons[i].GetComponent<Transform>().name;
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
            Debug.Log(IconName);
        }
    }

    void Clicked_Chat(BaseEventData eventData)
    {

            GameObject obj = (GameObject)Resources.Load("InputChat");
            Transform Canvas = GameObject.Find("Canvas").transform;
            Instantiate(obj, new Vector3(1227, 541, 0), Quaternion.identity, Canvas);

            PlayerPrefs.SetInt("State", 1);
            obj = GameObject.Find("InputChat(Clone)/Send");
            Send = obj.GetComponent<Button>();

            Send.onClick.AddListener(() => { SendChat(); });
    }

    void Clicked_Menu(BaseEventData eventData)
    {
        GameObject obj = (GameObject)Resources.Load("PlayMenu");
        Transform Canvas = GameObject.Find("Canvas").transform;
        Instantiate(obj, new Vector3(1227, 541, 0), Quaternion.identity, Canvas);
        PlayerPrefs.SetInt("State", 1);
    }

    void SendChat()
    {
        GameObject parent = GameObject.Find("InputChat(Clone)");
        Destroy(parent);
        PlayerPrefs.SetInt("State", 0);
    }
}
