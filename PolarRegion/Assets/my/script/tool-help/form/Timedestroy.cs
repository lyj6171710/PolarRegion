using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timedestroy : MonoBehaviour
{
    public float wait_time;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, wait_time);
    }
}
