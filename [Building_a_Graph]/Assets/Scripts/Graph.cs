using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;

    private void Awake()
    {
        Instantiate(this.pointPrefab);
    }
}
