using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureWear : MonoBehaviour,ITreeBranch
{//负责所有装备，但不包括武器

    void Ready()
    {
    }

    public List<AttrBaseEquip> GetPropertyEachWear()
    {
        return new List<AttrBaseEquip>();
    }

    public void SelfReady(TreeBranch shelf)
    {
        Ready();
    }

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        source = 0;
        return null;
    }

    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }
}
