using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicWindow : MonoBehaviour
{
    public virtual void ShowWindow()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideWindow()
    {
        gameObject.SetActive(false);
    }
}
