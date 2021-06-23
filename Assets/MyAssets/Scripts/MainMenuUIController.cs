using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MainMenuUIController : MonoBehaviour
{
    GameObject[] menu_elem;
    GameObject Canvas;
    PlayMenuController playMenuController;
    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        GameObject menu_base = GameObject.Find("menu_base");
        menu_elem = new GameObject[menu_base.transform.childCount];
        for(int i = 0;i < menu_elem.Length;i++)
        {
            menu_elem[i] = menu_base.transform.GetChild(i).gameObject;
            print(menu_elem[i].name);

            EventTrigger eventTrigger = menu_elem[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((BaseEventData eventData) => {
                Clicked_elem(i);
            });
            eventTrigger.triggers.Add(entry);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Clicked_elem(int a)
    {
        switch (a)
        {
            case 0:

                break;
            case 1:
                break;
            case 2:
                GameObject obj = (GameObject)Resources.Load("SettingUI");
                Instantiate(obj,Canvas.transform.position,Quaternion.identity,Canvas.transform);
                obj = GameObject.Find("SettingUI(Clone)");
                Button[] buttons = new Button[obj.transform.Find("Buttons").childCount];
                for(int j = 0;j < buttons.Length; j++)
                {
                    buttons[j] = obj.transform.GetChild(j).GetComponent<Button>();
                    buttons[j].onClick.AddListener(() => {
                        switch (j)
                        {
                            case 0:
                                ChangeName();
                                break;
                            case 1:
                                //操作方法クリックされた際の処理
                                obj = (GameObject)Resources.Load("Help");
                                Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);
                                break;
                            case 2:
                                //アバター編集用のUIを呼び出す
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
                                break;
                            default:
                                break;
                        }
                    });
                }
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    private void ChangeName()
    {
        //ユーザーのなまえの変更（AWS側の処理）
        throw new NotImplementedException();
    }

    void Clicked_Man(GameObject UIbase)
    {
        //クリックされた際の処理
        PlayerPrefs.SetString("sex", "man");
        //GameObject obj = GameObject.Find("Edit_Model(Clone)");
        Destroy(this.gameObject);
        UIbase.active = true;
    }

    void Clicked_Woman(GameObject UIbase)
    {
        //クリックされた際の処理
        PlayerPrefs.SetString("sex", "woman");
        //GameObject obj = GameObject.Find("Edit_Model(Clone)");
        Destroy(this.gameObject);
        UIbase.active = true;
    }
}
