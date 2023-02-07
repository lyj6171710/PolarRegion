using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjPool : GbjPool, ISwitchScene
{

    public List<GameObject> mPreBuildsDp;

    //==============================

    public static GameobjPool It;

    public void WhenAwake()
    {
        It = this;
        Initial(null, mPreBuildsDp);
    }

    public void WhenSwitchScene()
    {

    }
}
