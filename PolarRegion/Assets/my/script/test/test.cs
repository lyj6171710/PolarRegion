using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;

public class test : MonoBehaviour
{
    HashSet<GameObject> lists;

    GameObject one { get { foreach (GameObject one in lists) return one;return null; } }

    private void Awake()
    {
        lists = new HashSet<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            lists.Add(SpritePool.It.GetPure(transform));
        else if (Input.GetKeyDown(KeyCode.O))
        {
            SpritePool.It.Revert(one);
            lists.Remove(one);
        }
    }

}
