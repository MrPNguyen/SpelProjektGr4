using UnityEngine;

public class Page : MonoBehaviour
{
    public int normalOrder = 0;
    public int flipOrder = 10;

    Canvas canvas;

    void Awake() {
        canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
    }
    
    public void BringToFront()
    {
        if (canvas != null)
            canvas.sortingOrder = flipOrder;
    }

    public void SendToBack()
    {
        if (canvas != null)
            canvas.sortingOrder = normalOrder;
    }
    
    public void ResetOrder() 
    {
        if (canvas != null)
        canvas.sortingOrder = normalOrder;
    }
}
