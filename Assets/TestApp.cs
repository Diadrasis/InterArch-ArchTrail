using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestApp : MonoBehaviour
{

    public Button btnOpen;
    public GameObject activeObject;
    // Start is called before the first frame update
    void Start()
    {
        activeObject.SetActive(false);
        btnOpen.onClick.AddListener(()=>OpenMenuButtons());
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPointerOverUIObject())
            OpenMenuButtons();
       
    }

    void OpenMenuButtons()
    {
        
            activeObject.SetActive(true);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
