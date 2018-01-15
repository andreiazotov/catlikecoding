using System;
using UnityEngine;

public class Clock : MonoBehaviour
{

    public Transform hoursTransform;
    public Transform minutesTransform;
    public Transform secondsTransform;
    public bool continuous;

    private const float DegreePerHour = 30.0f;
    private const float DegreePerMinute = 6.0f;
    private const float DegreePerSecond = 6.0f;

    private void Update()
    {
        /*
        * Time.time show how many
        * seconds passed after
        * entering play mode
        * 
        * Debug.Log(Time.time);
        */

        if (this.continuous)
        {
            this.UpdateContinuos();
        }
        else
        {
            this.UpdateDiscrete();
        }


    }

    private void UpdateContinuos()
    {
        var time = DateTime.Now.TimeOfDay;
        this.hoursTransform.localRotation = Quaternion.Euler(0.0f, (float)time.TotalHours * DegreePerHour, 0.0f);
        this.minutesTransform.localRotation = Quaternion.Euler(0.0f, (float)time.TotalMinutes * DegreePerMinute, 0.0f);
        this.secondsTransform.localRotation = Quaternion.Euler(0.0f, (float)time.TotalSeconds * DegreePerSecond, 0.0f);
    }

    private void UpdateDiscrete()
    {
        var time = DateTime.Now;
        this.hoursTransform.localRotation = Quaternion.Euler(0.0f, time.Hour * DegreePerHour, 0.0f);
        this.minutesTransform.localRotation = Quaternion.Euler(0.0f, time.Minute * DegreePerMinute, 0.0f);
        this.secondsTransform.localRotation = Quaternion.Euler(0.0f, time.Second * DegreePerSecond, 0.0f);
    }
}
