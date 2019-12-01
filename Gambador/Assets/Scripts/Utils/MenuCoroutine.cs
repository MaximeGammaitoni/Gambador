using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCoroutine : MonoBehaviour
{
    private GameObject Title;
    private GameObject Credits;
    private Text creditsText;
    private string text1 = "Twin Gears present";
    private string text2 = "A GitHub GameOff 2K19 Game";
    private string text3 = "Code and GameDesign by John Touba and Maxime Gammaitoni \n \n Art by Mathieu Strzykala and Denis Krozcek \n  \n  Music by Alexis Imperial";
    void Start()
    {
        Credits = GameObject.Find("Credits");
        creditsText = Credits.transform.Find("Text").GetComponent<Text>();
        Title = GameObject.Find("Title");
        StartCoroutine(StartAnims());
    }

    // Update is called once per frame
    IEnumerator StartAnims()
    {
        Title.SetActive(false);
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
        Title.SetActive(true);

        yield return new WaitForSeconds(2);
        //Title.GetComponent<Animator>().enabled = true;
    }
}
