using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IfoRay
{
    public Transform emitFrom;
    public float spanDistance;
    public Vector2 toward;
    public float validDistance;
    public string searchLayer;
}

public class RayCatch : MonoBehaviour
{
    //使用前提：人面向屏幕，屏幕中所显示场景使用的坐标系，以正右方为零度，逆时针旋转为正向，到正上方为九十度

    static RaycastHit2D InspectFirstByRay(IfoRay ray)
    {
        Vector2 emit_source = MathAngle.CoordTransfer(ray.emitFrom.position, ray.toward, ray.spanDistance);
        RaycastHit2D temp_hit;
        temp_hit = Physics2D.Raycast(emit_source, ray.toward, ray.validDistance, LayerMask.NameToLayer(ray.searchLayer));
        Debug.DrawLine(emit_source, MathAngle.CoordTransfer(emit_source, ray.toward, ray.validDistance), Color.red, 0.2f);//可检测射线具体状态
        return temp_hit;
    }

    static List<RaycastHit2D> InspectAllByRay(IfoRay ray)
    {
        Vector2 emit_source = MathAngle.CoordTransfer(ray.emitFrom.position, ray.toward, ray.spanDistance);
        RaycastHit2D[] temp_hit;
        temp_hit = Physics2D.RaycastAll(emit_source, ray.toward, ray.validDistance);
        Debug.DrawLine(emit_source,MathAngle.CoordTransfer(emit_source, ray.toward, ray.validDistance), Color.red, 0.2f);//可检测射线具体状态
        List<RaycastHit2D> result_hit = new List<RaycastHit2D>();
        for (int i = 0; i < temp_hit.Length; i++)
        {
            if (temp_hit[i].transform.gameObject.layer == LayerMask.NameToLayer(ray.searchLayer))
            {
                result_hit.Add(temp_hit[i]);
            }
        }
        return result_hit;
    }

    //----------------------------------------------

    public static bool SuRayInspectAll(IfoRay ray, ref RaycastHit2D hit)
    {//返回真时，所返回碰撞体对象才有效且具有所要求有的属性值
        List<RaycastHit2D> result_hit = InspectAllByRay(ray);
        if (result_hit.Count > 0)
        {
            hit = result_hit[0];
            return true;
        }
        else
            return false;
    }

    public static bool SuRayInspectAll(IfoRay ray)
    {//射线穿透性检测，如果碰撞到具有指定属性值的物体，返回真
     //注意射线检测所能检测到的物体至少需要有触发器，不是提供有参数就可以了

        List<RaycastHit2D> result_hit = InspectAllByRay(ray);
        for (int i = 0; i < result_hit.Count; i++)
        {
            if (result_hit[i].transform == ray.emitFrom)
                continue;//自己不会被检测
            else
                return true;//也不是就会检测所有，一旦有就返回了
        }
        return false;//所有被碰撞者都没能符合时
    }

    public static bool SuRayInspectAll(IfoRay ray, ref List<RaycastHit2D> hit)
    {//返回真时，所返回碰撞体对象才有效且具有所要求有的属性值
        hit = InspectAllByRay(ray);
        return hit.Count > 0 ? true : false;
    }

    //-------------------------------------------------

    public static bool SuRayInspectFirst(IfoRay ray)
    {//射线非穿透性检测，如果碰撞到具有指定属性的物体，返回真
     //有时会碰撞检测到自身，可以跨越一定距离再发出射线
     //有时会发现检测不到，因为该函数寻访的是第一个被碰撞到的物体，之后的物体就算被射线穿过也检测不到
        RaycastHit2D temp_hit =InspectFirstByRay(ray);
        if (temp_hit)
            return true;
        else 
            return false;
    }

    public static float SuRaySymmetryInspectAll(IfoRay ray)
    {
        if (SuRayInspectAll(ray))
            return ray.toward.x;
        ray.toward = MathAngle.CoordXAgainst(ray.toward);
        if (SuRayInspectAll(ray))
            return ray.toward.x;
        return 0;
    }

    public static float SuRaySymmetryInspectFirst(IfoRay ray)
    {
        if (SuRayInspectFirst(ray))
            return ray.toward.x;
        ray.toward = MathAngle.CoordXAgainst(ray.toward);
        if (SuRayInspectFirst(ray))
            return ray.toward.x;
        return 0;
    }
}
