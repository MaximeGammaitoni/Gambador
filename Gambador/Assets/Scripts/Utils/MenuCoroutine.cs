using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCoroutine : MonoBehaviour
{
    private bool canClick = false;
    private GameObject Title;
    private GameObject Credits;
    private GameObject BG;
    private GameObject click;
    private GameObject trans;
    private GameObject nowL;
    private Text creditsText;
    private string text1 = "Twin Gears present";
    private string text2 = "A GitHub GameOff 2K19 Game";
    private string text3 = "Code and GameDesign by John Touba and Maxime Gammaitoni \n \n Art by Mathieu Strzykala and Denis Krozcek \n  \n  Music by Alexis Imperial";
    void Start()
    {

        Credits = GameObject.Find("Credits");
        nowL = GameObject.Find("NowL");
        trans = GameObject.Find("Trans");
        click = GameObject.Find("Click");
        BG = GameObject.Find("BG");
        creditsText = Credits.transform.Find("Text").GetComponent<Text>();
        Title = GameObject.Find("Title");
        StartCoroutine(StartAnims());
    }

    // Update is called once per frame
    IEnumerator StartAnims()
    {
        Title.SetActive(false);
        trans.SetActive(false);
        nowL.SetActive(false);
        BG.GetComponent<Animator>().enabled = false;
        click.GetComponent<Animator>().enabled = false;
        creditsText.text = "";
        var index = 0;
        while (creditsText.text != text1)
        {
            creditsText.text += text1[index];
            yield return new WaitForSeconds(0.05f);
            index++;
        }
        yield return new WaitForSeconds(0.4f);
        while (creditsText.text != "")
        {
            creditsText.text = creditsText.text.Remove(creditsText.text.Length -1);
            yield return new WaitForSeconds(0.04f);
            index--;
        }
        yield return new WaitForSeconds(0.4f);
         index = 0;
        while (creditsText.text != text2)
        {
            creditsText.text += text2[index];
            yield return new WaitForSeconds(0.05f);
            index++;
        }

        yield return new WaitForSeconds(0.4f);
        while (creditsText.text != "")
        {
            creditsText.text = creditsText.text.Remove(creditsText.text.Length - 1);
            yield return new WaitForSeconds(0.04f);
            index--;
        }
        yield return new WaitForSeconds(0.4f);
        index = 0;
        creditsText.fontSize =28;
        while (creditsText.text != text3)
        {
            creditsText.text += text3[index];
            yield return new WaitForSeconds(0.03f);
            index++;
        }
        Credits.GetComponent<Animator>().SetBool("Close", true);
        
        yield return new WaitForSeconds(2f);
        BG.GetComponent<Animator>().enabled = true;
        Title.SetActive(true);
        BG.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        click.GetComponent<Animator>().enabled = true;
        canClick = true;
        yield return new WaitForSeconds(2);
        //Title.GetComponent<Animator>().enabled = true;
    
    }
    private void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            trans.SetActive(true);
            nowL.SetActive(true);
            StartCoroutine(nowLoadingC());
        }
    }

    IEnumerator nowLoadingC()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(1);
        while (true) 
        {
            nowL.GetComponent<Text>().text += ".";
            yield return new WaitForSeconds(1);
        }
    }
}
