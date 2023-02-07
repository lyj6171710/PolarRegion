using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsSource : MonoBehaviour
{
    public MapOneWorld mWorldDp;

    public static MapsSource It;

    public void MakeReady()
    {
        It = this;

        mWorldDp.MakeReady();
    }
}
