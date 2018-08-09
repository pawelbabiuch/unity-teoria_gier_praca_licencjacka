using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmenuBtnSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color unselect, select;
    public bool closeAfterClick = true;


    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = select;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().color = unselect;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (closeAfterClick)
            transform.parent.parent.GetComponent<SubmenuController>().OnPointerExit(eventData);
    }
}
