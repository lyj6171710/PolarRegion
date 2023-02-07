using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniformUiChildren : MonoBehaviour,AssistClick.IClick,AssistClick.ITouch {//该组件动态生成，协助批量生成同类型子元素

    //外界可用===========================

    //一次性赋值=========================
    
    //私用变量===========================

    List<RectTransform> units;//承载父子状态
    GameObject unit_refer;

    //内部机制===========================


    //组合可用============================

    public List<RectTransform> make_present_(int count, GameObject unit_refer, Transform unit_parent = null)
    {//包含对原有内容的删除
        this.unit_refer = unit_refer;
        if (unit_parent == null) unit_parent = transform;//如果没有设置归属，则认为归属于该组件所挂载的物体上
        if (units == null) units = new List<RectTransform>();
        else if (units.Count > 0) clear_all_unit_();//相对零子元素的状态
        for (int i = 0; i < count; i++)
        {
            RectTransform unit = form_a_unit_(i).GetComponent<RectTransform>();
            unit.transform.SetParent(unit_parent, false);
            units.Add(unit);
        }
        return units;//外界也能引用这一系列子级元素
    }

    //内部接口=============================

    GameObject form_a_unit_(int index)//形成一个能直观表达数据状态的物体
    {
        GameObject unit_sign = Instantiate(unit_refer);
        AssistClick react = unit_sign.AddComponent<AssistClick>();
        react.SuReactClick(this, index);
        react.SuReactTouch(this);
        return unit_sign;
    }

    void clear_all_unit_()//清除所有格子
    {
        for (int i = 0; i < units.Count;)
        {
            Destroy(units[i].gameObject);
            units.RemoveAt(i);
        }
    }

    //=======================================

    public void iClickFmConfirmPress(object para)
    {
        Debug.Log((int)para);
    }

    public void iHoverInsideFmMouse(object para)
    {
    }

    public void iHoverOutsideFmMouse(object para)
    {
    }
}
