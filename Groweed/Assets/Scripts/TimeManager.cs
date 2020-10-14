using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance { get; protected set; }
    float seconds;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        seconds += Time.deltaTime;
    }

    public void getFormattedTime()
    {
        int gameSeconds = Mathf.CeilToInt(seconds * 0.25f);
        
    }

    public void getFormattedDate()
    {

    }
}
