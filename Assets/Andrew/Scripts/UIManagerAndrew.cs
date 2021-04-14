using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerAndrew : MonoBehaviour
{
    #region Variables
    public GameObject availableAreasScreen;
    public GameObject mapScreen;
    public GameObject selectAreaPrefab;
    #endregion

    #region Unity Functions
    #endregion

    #region Methods
    public void Escape()
    {
        Application.Quit();
    }

    public void DisplayAvailableAreas()
    {
        // Get areas list from MapManager through AppManager
        List<cArea> areas = AppManager.Instance.mapManager.areas;

        foreach (cArea area in areas)
        {

        }
        // loop through 
    }
    #endregion
}
