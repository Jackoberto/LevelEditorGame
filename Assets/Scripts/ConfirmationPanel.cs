using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    public Text confirmText;
    public event Action OnConfirm;

    private void Start()
    {
        var exists = FindObjectsOfType<ConfirmationPanel>();
        foreach (var confirmation in exists)
        {
            if (confirmation != this)
                Destroy(confirmation.gameObject);
        }
    }

    public void SetUp(string text)
    {
        confirmText.text = text;
    }

    public void Confirm()
    {
        OnConfirm?.Invoke();
        Destroy(gameObject);
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }
}