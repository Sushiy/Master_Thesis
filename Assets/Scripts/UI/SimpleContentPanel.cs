using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleContentPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public void SetContent(string title, string content)
    {
        this.title.text = title;
        this.content.text = content;
    }
}
