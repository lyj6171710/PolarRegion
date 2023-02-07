using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PR_RunSys : MonoBehaviour
{
    public MapShowMager mShowMap;

    public MapsSource mDataMap;

    private void Awake()
    {
        mDataMap.MakeReady();
        mShowMap.MakeReady();
    }

    private void Start()
    {
        mShowMap.MakeUse();
    }

}
