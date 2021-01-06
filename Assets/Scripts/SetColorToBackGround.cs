using UnityEngine;
using UnityEngine.UI;

public class SetColorToBackGround : MonoBehaviour
{
    private void Start()
    {
        var color = Camera.main.backgroundColor;
        color.a = 255;
        GetComponent<Image>().color = color;
    }
}
