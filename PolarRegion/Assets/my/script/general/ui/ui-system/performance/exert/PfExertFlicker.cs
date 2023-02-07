using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PfExertFlicker : MonoBehaviour
{
    PfChangeElemOpacity mOpacity;

    private void Awake()
    {
        mOpacity = GetComponent<PfChangeElemOpacity>();
    }

    private void Update()
    {
        if (TimeUse.It.SuIfJustPastSpecGap(2))
        {
            mOpacity.meMakeForth = true;
            mOpacity.meMakeBack = true;
        }
    }

}
