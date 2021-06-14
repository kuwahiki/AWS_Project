using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayMenuController : MonoBehaviour
{
    GameObject menu_base, icons_parent;
    GameObject[] icons;
    // Start is called before the first frame update
    void Start()
    {

        //Menu＿baseのStart処理
        menu_base = GameObject.Find("Menu_base");
        menu_base.transform.localScale = Vector3.zero;
        menu_base.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 1.0f).SetEase(Ease.OutBounce);

        //アイコンのStart処理
        icons_parent = GameObject.Find("icons");
        icons = new GameObject[icons_parent.transform.childCount];

        for(int i = 0; i < icons.Length; i++)
        {
            icons[i] = icons_parent.transform.GetChild(i).gameObject;
            string ObjectName = icons[i].name;
            //Debug.Log(ObjectName);

            EventTrigger trigger = icons[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            // なんのイベントを検出するか
            entry.eventID = EventTriggerType.PointerClick;
            // コールバック登録
            switch (ObjectName)
            {
                case "Icon_back":
                    entry.callback.AddListener(Clicked_back);
                    break;
                case "Icon_delete":
                    entry.callback.AddListener(Clicked_delete);
                    break;
                case "Icon_help":
                    entry.callback.AddListener(Clicked_help);
                    break;
                case "Icon_info":
                    entry.callback.AddListener(Clicked_info);
                    break;
                case "Icon_Tool":
                    entry.callback.AddListener(Clicked_Tool);
                    break;
                case "Icon_chat":
                    entry.callback.AddListener(Clicked_chat);
                    break;
                default:
                    break;
            }
            // EventTriggerに追加
            trigger.triggers.Add(entry);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(this.GetComponent<CanvasGroup>().alpha <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("State", 0);
    }

    void Clicked_back(BaseEventData eventData)
    {
        //クリックされた際の処理
        this.GetComponent<CanvasGroup>().DOFade(0, 0.6f).SetEase(Ease.InOutExpo);
    }

    void Clicked_delete(BaseEventData eventData)
    {
        //クリックされた際の処理
        icons_parent.active = false;
        GameObject obj = (GameObject)Resources.Load("Logout");
        Instantiate(obj, new Vector3(1227, 541, 0), Quaternion.identity, menu_base.transform);

        //ログアウトする場合
        Button yes = GameObject.Find("Yes").GetComponent<Button>();
        yes.onClick.AddListener(() => { Logout(); });

        //ログアウトしない場合
        Button No = GameObject.Find("No").GetComponent<Button>();
        No.onClick.AddListener(() =>
        {
            GameObject obj = GameObject.Find("Logout(Clone)");
            Destroy(obj);
            icons_parent.active = true;
        });
    }

    void Clicked_help(BaseEventData eventData)
    {
        //クリックされた際の処理
        GameObject obj = (GameObject)Resources.Load("Help");
        Transform Canvas = GameObject.Find("Canvas").transform;
        Instantiate(obj, new Vector3(1227, 541, 0), Quaternion.identity, Canvas);
    }

    void Clicked_info(BaseEventData eventData)
    {
        //クリックされた際の処理
        icons_parent.active = false;
        GameObject obj = (GameObject)Resources.Load("Edit_Model");
        Instantiate(obj, new Vector3(1227, 541, 0), Quaternion.identity, menu_base.transform);

        Image[] images = new Image[2];
        images[0] = GameObject.Find("Edit_Model(Clone)/Man").GetComponent<Image>();
        images[1] = GameObject.Find("Edit_Model(Clone)/Woman").GetComponent<Image>();

        for(int i = 0; i < images.Length; i++)
        {
            EventTrigger trigger = images[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            // なんのイベントを検出するか
            entry.eventID = EventTriggerType.PointerClick;
            // コールバック登録
            switch (i)
            {
                case 0:
                    entry.callback.AddListener(Clicked_Man);
                    break;
                case 1:
                    entry.callback.AddListener(Clicked_Woman);
                    break;
                default:
                    break;
            }
            // EventTriggerに追加
            trigger.triggers.Add(entry);
        }
    }   

    void Clicked_Tool(BaseEventData eventData)
    {
        //クリックされた際の処理
    }

    void Clicked_chat(BaseEventData eventData)
    {
        //クリックされた際の処理
    }

    void Clicked_Man(BaseEventData eventData)
    {
        //クリックされた際の処理
        PlayerPrefs.SetString("sex","man");
        GameObject obj = GameObject.Find("Edit_Model(Clone)");
        Destroy(this.gameObject);
        icons_parent.active = true;
    }

    void Clicked_Woman(BaseEventData eventData)
    {
        //クリックされた際の処理
        PlayerPrefs.SetString("sex", "woman");
        GameObject obj = GameObject.Find("Edit_Model(Clone)");
        Destroy(this.gameObject);
        icons_parent.active = true;
    }
    void Logout()
    {
        //ログアウトの処理（AWS側）
    }
}
