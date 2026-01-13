using System.Collections.Generic;
using UnityEngine;

public class PageList : MonoBehaviour
{
    public List<string> pages;
    public static PageList instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);    
    }
}
