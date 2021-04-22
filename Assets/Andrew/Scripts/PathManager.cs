using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    #region Variables
    #endregion

    #region Unity Functions
    #endregion

    #region Methods
    /// <summary>
    /// load all saved paths with button list
    /// in order to select paths to view on map
    /// </summary>
    /*public void LoadSavedPathsButtonList()
    {
        if (PlayerPrefs.HasKey(savedPathsKey))
        {
            pathNamesSavedList = PlayerPrefsX.GetStringArray(savedPathsKey).ToList();
        }

        if (pathNamesSavedList.Count <= 0)
        {
            ShowMessage("There are no saved paths", 2f);
            return;
        }


        pathNamesToShow.Clear();

        List<Button> pathButtons = containerSavedPaths.GetComponentsInChildren<Button>(true).ToList();

        for (int i = 0; i < pathNamesSavedList.Count; i++)
        {
            bool isPhotosList = pathImagesSavedList.Count > i && serverManager.useScreenShots;


            string pathName = pathNamesSavedList[i];

            if (pathName.Contains("---"))
            {
                int indx = pathName.IndexOf("-");
                pathName = pathName.Substring(0, indx);
            }

            if (pathNamesToShow.Contains(pathName)) { continue; }

            if (i < pathButtons.Count)
            {
                pathButtons[i].gameObject.SetActive(true);

                Image img = pathButtons[i].image;
                Text txt = pathButtons[i].GetComponentInChildren<Text>();
                Image tick = pathButtons[i].transform.Find("tick").GetComponent<Image>();
                tick.color = Color.white;

                txt.text = i + " (" + pathName + ")";
                pathButtons[i].onClick.RemoveAllListeners();
                pathButtons[i].onClick.AddListener(() => ButtonPathSelection(tick, pathName));

                Transform ft = pathButtons[i].transform.Find("foto");

                //check if it has foto and show the correct prefab
                if (isPhotosList && pathImagesSavedList[i] != null && pathImagesSavedList[i].width > 8)
                {
                    //Debug.Log(pathImagesSavedList[i].width);

                    if (ft)
                    {
                        RawImage foto = ft.GetComponent<RawImage>();

                        if (foto)
                        {
                            foto.texture = pathImagesSavedList[i];
                            Vector2 size = new Vector2(foto.texture.width, foto.texture.height);
                            AspectRatioFitter asp = foto.GetComponent<AspectRatioFitter>();
                            asp.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                            asp.aspectRatio = size.x / size.y;
                        }
                    }
                    else
                    {
                        pathButtons[i].gameObject.SetActive(false);

                        Transform btnTr = Instantiate(Resources.Load<Transform>("Prefabs/UI/PathButton_big") as Transform, containerSavedPaths);
                        RawImage foto = btnTr.Find("foto").GetComponent<RawImage>();
                        foto.texture = pathImagesSavedList[i];
                        Vector2 size = new Vector2(foto.texture.width, foto.texture.height);
                        AspectRatioFitter asp = foto.GetComponent<AspectRatioFitter>();
                        asp.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                        asp.aspectRatio = size.x / size.y;

                        Button btn = btnTr.GetComponent<Button>();

                        //Image img = btn.image;
                        txt = btn.GetComponentInChildren<Text>();
                        tick = btnTr.Find("tick").GetComponent<Image>();
                        tick.color = Color.white;

                        txt.text = i + " (" + pathName + ")";

                        btn.onClick.AddListener(() => ButtonPathSelection(tick, pathName));
                    }
                }
                else
                {
                    if (!ft)
                    {
                        pathButtons[i].gameObject.SetActive(false);

                        Transform btnTr = Instantiate(Resources.Load<Transform>("Prefabs/UI/PathButton_small") as Transform, containerSavedPaths);

                        Button btn = btnTr.GetComponent<Button>();

                        //Image img = btn.image;
                        txt = btn.GetComponentInChildren<Text>();
                        tick = btnTr.Find("tick").GetComponent<Image>();
                        tick.color = Color.white;

                        txt.text = i + " (" + pathName + ")";

                        btn.onClick.AddListener(() => ButtonPathSelection(tick, pathName));
                    }
                }
            }
            else//instatiate
            {
                Transform btnTr;

                if (!isPhotosList || pathImagesSavedList[i] == null || pathImagesSavedList[i].width == 8)
                {
                    btnTr = Instantiate(Resources.Load<Transform>("Prefabs/UI/PathButton_small") as Transform, containerSavedPaths);
                }
                else
                {
                    btnTr = Instantiate(Resources.Load<Transform>("Prefabs/UI/PathButton_big") as Transform, containerSavedPaths);
                    RawImage foto = btnTr.Find("foto").GetComponent<RawImage>();
                    foto.texture = pathImagesSavedList[i];
                    Vector2 size = new Vector2(foto.texture.width, foto.texture.height);
                    AspectRatioFitter asp = foto.GetComponent<AspectRatioFitter>();
                    asp.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    asp.aspectRatio = size.x / size.y;
                }

                // Transform btnTr = Instantiate(Resources.Load<Transform>("Prefabs/UI/PathButton")as Transform, containerSavedPaths);

                Button btn = btnTr.GetComponent<Button>();

                //Image img = btn.image;
                Text txt = btn.GetComponentInChildren<Text>();
                Image tick = btnTr.Find("tick").GetComponent<Image>();
                tick.color = Color.white;

                txt.text = i + " (" + pathName + ")";

                btn.onClick.AddListener(() => ButtonPathSelection(tick, pathName));
            }
        }

        if (pathButtons.Count > pathNamesSavedList.Count)
        {
            for (int x = pathNamesSavedList.Count; x < pathButtons.Count; x++)
            {
                pathButtons[x].gameObject.SetActive(false);
            }
        }

        //ShowSettings(true);
        pGeneral.SetActive(false);
        pSavedPaths.SetActive(true);

    }*/
    #endregion
}