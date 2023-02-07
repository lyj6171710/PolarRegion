using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCatch
{

    public static Collider2D[] SuAffectCircle(string filter, Vector3 center, float radius)
    {
        LayerMask aimMask = 1 << (LayerMask.NameToLayer(filter));
        return Physics2D.OverlapCircleAll(center, radius, aimMask.value);
    }

    //--------------------------------------------

    static void RectToNormal(Transform emit_from, float span_distance, Vector2 toward, float width, float length, out float central_angle, out Vector2 shape, out Vector2 emit_source)
    {
        central_angle = Vector2.Angle(Vector2.right, toward);
        shape = new Vector2(length, width);
        emit_source = Vector2.zero;
        if (toward.x > 0)
            emit_source = new Vector2(emit_from.position.x + length / 2, emit_from.position.y);
        if (toward.x < 0)
            emit_source = new Vector2(emit_from.position.x - length / 2, emit_from.position.y);
        emit_source = MathAngle.CoordTransfer(emit_source, toward, span_distance);
    }

    static RaycastHit2D InspectFirstByRect(Transform emit_from, float span_distance, Vector2 toward, float width, float length, string search_layer)
    {
        float central_angle;
        Vector2 shape;
        Vector2 emit_source;
        RectToNormal(emit_from, span_distance, toward, width, length, out central_angle, out shape, out emit_source);
        return Physics2D.BoxCast(emit_source, shape, central_angle, toward, 0, LayerMask.NameToLayer(search_layer));
    }

    static List<RaycastHit2D> InspectAllByRect(Transform emit_from, float span_distance, Vector2 toward, float width, float length, string search_layer)
    {
        float central_angle;
        Vector2 shape;
        Vector2 emit_source;
        RectToNormal(emit_from, span_distance, toward, width, length, out central_angle, out shape, out emit_source);
        RaycastHit2D[] temp_hit;
        temp_hit = Physics2D.BoxCastAll(emit_source, shape, central_angle, toward, 0);
        List<RaycastHit2D> result_hit = new List<RaycastHit2D>();
        for (int i = 0; i < temp_hit.Length; i++)
        {
            if (temp_hit[i].transform.gameObject.layer == LayerMask.NameToLayer(search_layer))
                result_hit.Add(temp_hit[i]);
        }
        return result_hit; ;
    }


    public static bool SuRectInspectAll(Transform emit_from, float span_distance, Vector2 toward, float width, float length, string search_layer)
    {
        List<RaycastHit2D> temp_hit = InspectAllByRect(emit_from, span_distance, toward, width, length, search_layer);
        if (temp_hit.Count > 0)
            return true;
        else
            return false;
    }
    public static bool SuRectInspectFirst(Transform emit_from, float span_distance, Vector2 toward, float width, float length, string search_layer)
    {
        RaycastHit2D temp_hit = InspectFirstByRect(emit_from, span_distance, toward, width, length, search_layer);
        if (temp_hit)
            return true;
        else
            return false;
    }
    public static float SuRectSymmetryInspectAll(Transform emit_from, float span_distance, Vector2 toward, float width, float length, string search_layer)
    {
        if (SuRectInspectAll(emit_from, span_distance, toward, width, length, search_layer))
            return toward.x;
        toward = MathAngle.CoordXAgainst(toward);
        if (SuRectInspectAll(emit_from, span_distance, toward, width, length, search_layer))
            return toward.x;
        return 0;
    }
    public static float SuRectSymmetryInspectFirst(Transform emit_from, float span_distance, Vector2 toward, float width, float length, string search_layer)
    {
        if (SuRectInspectFirst(emit_from, span_distance, toward, width, length, search_layer))
            return toward.x;
        toward = MathAngle.CoordXAgainst(toward);
        if (SuRectInspectFirst(emit_from, span_distance, toward, width, length, search_layer))
            return toward.x;
        return 0;
    }
}
