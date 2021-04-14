using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManagerAndrew : MonoBehaviour
{
    #region Variables
    public GameObject availableAreasScreen;
    public GameObject mapScreen;
    public GameObject selectAreaPrefab;
    public GameObject availableAreasPanel;

    private List<GameObject> selectAreaObjects;
    private float interval = 0.001f;
    #endregion

    #region Unity Functions
    public void Start()
    {
        ResetSelectAreaObjects(selectAreaObjects);
        selectAreaObjects = InstantiateAvailableAreas();
        StartCoroutine(ReloadLayout(availableAreasPanel));
    }
    #endregion

    #region Methods
    public void Escape()
    {
        Application.Quit();
    }

    public List<GameObject> InstantiateAvailableAreas()
    {
        List<GameObject> newSelectAreaObjects = new List<GameObject>();
        List<cArea> areas = AppManager.Instance.mapManager.areas;

        foreach (cArea area in areas)
        {
            GameObject newSelectArea = Instantiate(selectAreaPrefab, Vector3.zero, Quaternion.identity, availableAreasPanel.GetComponent<RectTransform>());
            //newSelectArea.transform.SetAsFirstSibling();
            TMP_Text selectAreaText = newSelectArea.GetComponentInChildren<TMP_Text>();
            selectAreaText.text = area.title;
            Button button = newSelectArea.GetComponentInChildren<Button>();
            //button.onClick.AddListener(AppManager.Instance.experimentManager.uIExperimentManager.OnStatementSelected);
            newSelectAreaObjects.Add(newSelectArea);
        }

        return newSelectAreaObjects;
    }

    private void ResetSelectAreaObjects(List<GameObject> _selectAreaObjects)
    {
        if (_selectAreaObjects != null)
        {
            foreach (GameObject selectArea in _selectAreaObjects)
            {
                Destroy(selectArea);
            }

            _selectAreaObjects.Clear();
        }

        _selectAreaObjects = new List<GameObject>();
    }

    IEnumerator ReloadLayout(GameObject _layoutGameObject)
    {
        yield return new WaitForSeconds(interval);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGameObject.GetComponent<RectTransform>());
    }
    #endregion
}
