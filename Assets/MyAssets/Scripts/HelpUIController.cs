using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HelpUIController : MonoBehaviour
{
    private Image Back;
    CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.01f;
        canvasGroup.DOFade(1, 1).SetEase(Ease.InOutExpo);

        string thisname = this.name;
        Back = GameObject.Find(name + "/back").GetComponent<Image>();

        EventTrigger trigger = Back.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();

        // なんのイベントを検出するか
        entry.eventID = EventTriggerType.PointerClick;
        // コールバック登録
        entry.callback.AddListener(Onclick_back);
        // EventTriggerに追加
        trigger.triggers.Add(entry);

    }

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup.alpha <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void Onclick_back(BaseEventData eventData)
    {
        this.canvasGroup.DOFade(0, 1).SetEase(Ease.InOutExpo);
    }
}
