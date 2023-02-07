using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CtrlChart : MonoBehaviour
{
    protected abstract void DealEachChild(GameObject child, int index);

    public virtual void MakeReady()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject pick = transform.GetChild(i).gameObject;
            pick.SetActive(false);
            DealEachChild(pick, i);
        }
    }
}
