using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TimeRecordUi : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private TextMeshProUGUI timeRecordText;


    private void Update()
    {
        float timeRecord = timer.time;
        
        int min = Mathf.FloorToInt(timeRecord / 60);
        int sec = Mathf.FloorToInt(timeRecord % 60);
        
        timeRecordText.text = string.Format("TimeRecord - {0:00}:{1:00}", min, sec);
    }
}
