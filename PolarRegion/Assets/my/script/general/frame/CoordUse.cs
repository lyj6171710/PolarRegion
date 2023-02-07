using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoordUse:MonoBehaviour,ISwitchScene 
{
    //ʹ��������Ϣ����ȡͨ������
    //������Ϣ��Ϊ��ͬ���࣬����ȡ������������꣬���ء����ڡ�������Ƭ��

    //���������ʹ��===============================

    public static bool SuWhetherInside(Vector2 location, RectTransform area, float pad = 0)
    {//��һ�����������ǻ�������λ��

        RectMeter cornerAt = new RectMeter();
        cornerAt.leftBottom = CoordFrame.SuGetCornerBL(area);
        cornerAt.rightTop = CoordFrame.SuGetCornerTR(area);

        return MathRect.SuWhetherInside(location, cornerAt, pad);
    }

    //���������ʹ��==================================

    public static GameObject SuCatchGameObject(Vector3 worldPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(worldPos);//���λ�õ����֮�������
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);//�Խ��㷢�����
        if (hit.transform)
            return hit.transform.gameObject;
        else
            return null;
    }

    //��Ļ�����ʹ��===================================

    public const string cReactCan = "respond";
    //����ô˸�ֵǰ����ȷ����ǩ�б����Ѿ����Ӹñ�ǩ
    public const string cReactMask = "operate-mask";

    public static bool SuIsOverUI(Vector2 screenPos, out List<RaycastResult> hits)
    {
        var eventData = new PointerEventData(EventSystem.current) { position = screenPos };

        hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);//��ȷ�������Ϸ�����eventSystem
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
                if (tag == cReactCan)//�����з�Ӧ��
                {
                    IClick hit = results[i].gameObject.GetComponent<IClick>();
                    if (hit != null)
                        return hit;//ֻȡ�����
                    else continue;
                }
                else if (tag == cReactMask)
                    return null;//˵�����ڵ����Ͱ�������
                else
                    continue;//Ĭ����ֱ�Ӵ�͸��
            }
        }

        return null;
    }

    //Collider CheckHitInScene(Vector2 screenPos)//��һ�ַ�����
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(screenPos);//����ʶ��
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
