using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TileSelectButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onLeftClick;
    public UnityEvent onRightClick;
    public UnityEvent onMiddleClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftClick.Invoke ();
                break;
            case PointerEventData.InputButton.Middle:
                onMiddleClick.Invoke ();
                break;
            case PointerEventData.InputButton.Right:
                onRightClick.Invoke ();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
