using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time = 0.0f;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        this.GetComponent<UnityEngine.UI.Text>().text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        //int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));  :{2:000}
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
