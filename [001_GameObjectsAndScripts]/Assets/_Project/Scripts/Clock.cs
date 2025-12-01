using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private const float DEGREE_PER_HOUR = 30.0f;
    private const float DEGREE_PER_MINUTE = 6.0f;
    private const float DEGREE_PER_SECOND = 6.0f;

    [SerializeField]
    private Transform _hours;

    [SerializeField]
    private Transform _minutes;

    [SerializeField]
    private Transform _seconds;

    private void Update()
    {
        var time = DateTime.Now.TimeOfDay;
        _hours.localRotation = Quaternion.Euler(0.0f, (float)time.TotalHours * DEGREE_PER_HOUR, 0.0f);
        _minutes.localRotation = Quaternion.Euler(0.0f, (float)time.TotalMinutes * DEGREE_PER_MINUTE, 0.0f);
        _seconds.localRotation = Quaternion.Euler(0.0f, (float)time.TotalSeconds * DEGREE_PER_SECOND, 0.0f);
    }
}
