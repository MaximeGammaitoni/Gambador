using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinTrigger : MonoBehaviour
{
    private GameObject WinGo;
    private Text timerText;
    void Start()
    {
        WinGo = GameObject.Find("Win");

        WinGo.SetActive(false);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col)
    {
        timerText = GameObject.Find("Timer").GetComponent<Text>();
        WinGo.SetActive(true);
        WinGo.transform.Find("YourTime").GetComponent<Text>().text += ""+timerText.text;
        StartCoroutine(ReturnMenu());
    }

    IEnumerator ReturnMenu()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(0);
        GameManager.singleton.DestroyServices();
    }
}
