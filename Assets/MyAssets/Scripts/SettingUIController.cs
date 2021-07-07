using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SettingUIController : MonoBehaviour
{
    Button[] buttons;
    CanvasGroup canvasGroup;
    GameObject Canvas,parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject;
        Canvas = GameObject.Find("Canvas");
        canvasGroup = parent.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.01f;
        canvasGroup.DOFade(1,1);

        GameObject obj = parent.transform.Find("UIBase/Buttons").gameObject;
        buttons = new Button[obj.transform.childCount];
        for(int i = 0;i < buttons.Length; i++)
        {
            buttons[i] = obj.transform.GetChild(i).GetComponent<Button>();
            switch (i)
            {
                case 0:
                    buttons[i].onClick.AddListener(() => {
                        InputField text = parent.transform.Find("UIBase/User_Input").GetComponent<InputField>();
                        ChangeName(text.text);
                        text.text = null;
                        text = null;
                    });
                    break;
                case 1:
                    //操作方法クリックされた際の処理
                    buttons[i].onClick.AddListener(() => {
                        obj = (GameObject)Resources.Load("Help");
                        Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);
                    });
                    break;
                case 2:
                    //アバター編集用のUIを呼び出す
                    buttons[i].onClick.AddListener(() => {
                        obj = (GameObject)Resources.Load("Edit_Model");
                        Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);
                        GameObject UIbase = GameObject.Find("UIBase");
                        UIbase.active = false;

                        Image[] images = new Image[2];
                        images[0] = GameObject.Find("Edit_Model(Clone)/Man").GetComponent<Image>();
                        images[1] = GameObject.Find("Edit_Model(Clone)/Woman").GetComponent<Image>();

                        for (int i = 0; i < images.Length; i++)
                        {
                            EventTrigger trigger = images[i].GetComponent<EventTrigger>();
                            EventTrigger.Entry entry = new EventTrigger.Entry();
                            // なんのイベントを検出するか
                            entry.eventID = EventTriggerType.PointerClick;
                            // コールバック登録
                            switch (i)
                            {
                                case 0:
                                    entry.callback.AddListener((BaseEventData eventData) => { Clicked_Man(UIbase); });
                                    break;
                                case 1:
                                    entry.callback.AddListener((BaseEventData eventData) => { Clicked_Woman(UIbase); });
                                    break;
                                default:
                                    break;
                            }
                            // EventTriggerに追加
                            trigger.triggers.Add(entry);
                        }
                    });
                    break;
                default:
                    break;
            }
        }

        obj = parent.transform.Find("UIBase/delete").gameObject;
        EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((BaseEventData baseData)=> {
            canvasGroup.DOFade(0, 1);
        });
        eventTrigger.triggers.Add(entry);
    }

    private void ChangeName(string name)
    {
        //ユーザーのなまえの変更（AWS側の処理）
        PlayerPrefs.SetString("Username",name);
        Debug.Log(PlayerPrefs.GetString("Username"));
    }

    void Clicked_Man(GameObject UIbase)
    {
        //クリックされた際の処理
        PlayerPrefs.SetString("sex", "man");
        //GameObject obj = GameObject.Find("Edit_Model(Clone)");
        Destroy(GameObject.Find("Edit_Model(Clone)"));
        UIbase.active = true;
    }

    void Clicked_Woman(GameObject UIbase)
    {
        //クリックされた際の処理
        PlayerPrefs.SetString("sex", "woman");
        //GameObject obj = GameObject.Find("Edit_Model(Clone)");
        Destroy(GameObject.Find("Edit_Model(Clone)"));
        UIbase.active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.canvasGroup.alpha <= 0)
        {
            Destroy(parent.gameObject);
        }
    }
}
