using UnityEngine;

public class BasicWindow : MonoBehaviour
{
    public virtual void ShowWindow()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void HideWindow()
    {
        gameObject.SetActive(false);
    }
}
