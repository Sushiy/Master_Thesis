using UnityEngine;

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
