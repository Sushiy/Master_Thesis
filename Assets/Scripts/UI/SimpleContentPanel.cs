using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleContentPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public virtual void SetContent(string title, string content)
    {
        if(!string.IsNullOrEmpty(title))
            this.title.text = title;
        if (!string.IsNullOrEmpty(content))
            this.content.text = content;
    }
}
