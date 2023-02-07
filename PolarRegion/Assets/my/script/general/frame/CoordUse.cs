using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoordUse:MonoBehaviour,ISwitchScene 
{
    //使用坐标信息，获取通用数据
    //坐标信息分为不同种类，这里取最常见的四种坐标，场地、窗口、画布、片区

    //画布坐标的使用===============================

    public static bool SuWhetherInside(Vector2 location, RectTransform area, float pad = 0)
    {//第一个参数，得是画布坐标位置

        RectMeter cornerAt = new RectMeter();
        cornerAt.leftBottom = CoordFrame.SuGetCornerBL(area);
        cornerAt.rightTop = CoordFrame.SuGetCornerTR(area);

        return MathRect.SuWhetherInside(location, cornerAt, pad);
    }

    //世界坐标的使用==================================

    public static GameObject SuCatchGameObject(Vector3 worldPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(worldPos);//获得位置到鼠标之间的射线
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);//对交点发出检测
        if (hit.transform)
            return hit.transform.gameObject;
        else
            return null;
    }

    //屏幕坐标的使用===================================

    public const string cReactCan = "respond";
    //外界用此赋值前，请确保标签列表中已经增加该标签
    public const string cReactMask = "operate-mask";

    public static bool SuIsOverUI(Vector2 screenPos, out List<RaycastResult> hits)
    {
        var eventData = new PointerEventData(EventSystem.current) { position = screenPos };

        hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);//请确保场景上放置了eventSystem
        return hits.Count > 0;
    }

    public static IClick SuCheckHit(Vector2 screenPos)
    {
        var results = new List<RaycastResult>();
        if (SuIsOverUI(screenPos, out results))
        {
            for (int i = 0; i < results.Count; i++)
            {
                string tag = results[i].gameObject.tag;
                if (tag == cReactCan)//可以有反应的
                {
                    IClick hit = results[i].gameObject.GetComponent<IClick>();
                    if (hit != null)
                        return hit;//只取最近的
                    else continue;
                }
                else if (tag == cReactMask)
                    return null;//说明有遮挡，就按不了了
                else
                    continue;//默认是直接穿透的
            }
        }

        return null;
    }

    //Collider CheckHitInScene(Vector2 screenPos)//另一种方法？
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(screenPos);//交互识别
    //    RaycastHit hit;
    //    bool hasHit = Physics.Raycast(ray, out hit);
    //    if (hasHit) return hit.collider;
    //    else return null;
    //}

    //===============================

    public static CoordUse It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {
        
    }

}
