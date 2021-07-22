using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitSplash : MonoBehaviour
{
    public Animator animAppLogo, animDiadrasisLogo/*, animUniLogo,ministry1logo, ministry2logo, animPanel*/;
    public Image mainLogo;
    public Sprite beforeSrpite, secondSprite;
    public GameObject menuPanel;

    public static int isStarted;

    private void Awake()
    {
        B.Init();
        //ServerManager.Instance.Init();

        //menuPanel.SetActive(false);
        if (isStarted == 0)
        {
            menuPanel.gameObject.SetActive(true);

            //Screen.orientation = ScreenOrientation.Landscape;
        }
    }

    IEnumerator Start()
    {
        isStarted++;

        if (isStarted > 1)
        {
            if (Screen.orientation != ScreenOrientation.AutoRotation)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }

            menuPanel.gameObject.SetActive(false);
            //menuPanel.SetActive(true);
            //animMenu.SetBool("color", true);
            yield break;
        }
        mainLogo.sprite = beforeSrpite;
        /*animPanel.SetBool("show", false);*/
        yield return new WaitForSeconds(1f);
        animAppLogo.SetBool("show", true);
        yield return new WaitForSeconds(1f);
        /*animEspaLogo.SetBool("show", true);
        yield return new WaitForSeconds(2f);
        animEspaLogo.SetBool("show", false);*/
        yield return new WaitForSeconds(0.7f);
        animDiadrasisLogo.SetBool("show", true);
        yield return new WaitForSeconds(2f);
        animDiadrasisLogo.SetBool("show", false);
        yield return new WaitForSeconds(1.2f);
        animAppLogo.SetBool("show", false);
        yield return new WaitForSeconds(1.2f);

        mainLogo.sprite = secondSprite;
        yield return new WaitForSeconds(1f);
        animAppLogo.SetBool("show", true);
        yield return new WaitForSeconds(1.2f);
        animDiadrasisLogo.SetBool("show", true);
        yield return new WaitForSeconds(3f);
        animDiadrasisLogo.SetBool("show", false);
        /*animUniLogo.SetBool("show", true);
        yield return new WaitForSeconds(2f);
        animUniLogo.SetBool("show", false);
        yield return new WaitForSeconds(0.7f);
        ministry1logo.SetBool("show", true);
        yield return new WaitForSeconds(2f);
        ministry1logo.SetBool("show", false);
        yield return new WaitForSeconds(0.7f);
        ministry2logo.SetBool("show", true);
        yield return new WaitForSeconds(2f);
        ministry2logo.SetBool("show", false);*/
        //menuPanel.SetActive(true);
        //yield return new WaitForSeconds(0.7f);
        animAppLogo.SetBool("show", false);
        Screen.orientation = ScreenOrientation.Portrait;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
        //animPanel.SetBool("show", false);
        //animMenu.SetBool("color", true);
        //yield return new WaitForSeconds(0.7f);
        //animPanel.gameObject.SetActive(false);
        yield break;
    }

}
