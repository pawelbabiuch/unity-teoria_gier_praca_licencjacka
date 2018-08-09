using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubmenuController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static GameObject currentSelected;

    public Color unselect, select;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == false) return;

        Unselect();
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unselect();
    }

    private void Unselect()
    {
        if (currentSelected == null) return;

        currentSelected.transform.Find("Submenu").gameObject.SetActive(false);
        currentSelected = null;
    }

    private void Select()
    {
        currentSelected = this.gameObject;

        currentSelected.transform.Find("Submenu").gameObject.SetActive(true);

        Text[] btnTxts = currentSelected.transform.Find("Submenu").GetComponentsInChildren<Text>();

        foreach(Text txt in btnTxts)
        {
            txt.color = unselect;
        }
    }
}
