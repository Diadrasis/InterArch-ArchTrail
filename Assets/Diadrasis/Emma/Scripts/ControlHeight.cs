using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlHeight : MonoBehaviour
{
    public RectTransform rectParent;
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float totalH = rectParent.rect.size.y / 2f;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, totalH);
        
    }
}
