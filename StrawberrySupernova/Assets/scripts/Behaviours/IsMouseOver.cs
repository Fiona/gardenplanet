using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class IsMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool isOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

}
