using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class MainMenuUIController : MonoBehaviour
{
    GameObject[] menu_elem;
    GameObject Canvas;
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
            switch (i)
            {
                case 0:
                    entry.callback.AddListener((BaseEventData eventData) => {
                     
                    });
                    break;
                case 1:
                    entry.callback.AddListener((BaseEventData eventData) => {
                       
                    });
                    break;
                case 2:
                    entry.callback.AddListener((BaseEventData eventData) => {
                    GameObject obj = (GameObject)Resources.Load("SettingUI");
                    Instantiate(obj, Canvas.transform.position, Quaternion.identity, Canvas.transform);
                    });
                    break;
                case 3:
                    entry.callback.AddListener((BaseEventData eventData) => {
                        Application.Quit();
                        UnityEditor.EditorApplication.isPlaying = false;
                    });
                    break;
                default:
                    break;
            }

            eventTrigger.triggers.Add(entry);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

}
