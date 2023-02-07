using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GButton : UiGenerate
{//不同generator所生成图像在显示上的关系，需要手动调整场景对象的层级，越下会越优先

    public static GButton It;

    protected override void AwakeOther()
    {
        It = this;
    }

    //外界可用=================================

    public GameObject Form(int id, string explain, AloneButton.IClick react, int style, UiAlone.Ifo refer, float size = 1)
    {
        AloneButton button = FormAlone(style, refer, size).gameObject.AddComponent<AloneButton>();
        button.AcceptBase(id, explain, react);
        return button.gameObject;
    }

}
