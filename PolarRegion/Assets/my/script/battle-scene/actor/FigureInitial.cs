using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFgrNode { balance, can_do, attr_attach, attr_basis, atk_low, wear_body, move, profile, skill }

public enum EFgrMsg { kill_render,start_low_atk,finish_low_atk,switch_face }

public class FigureInitial : MonoBehaviour, ITreeFocus, ITreePeak
{//��������ٿ�����Ļ��ƣ����̲��費������ɻ���

    public ECamp mCamp;
    [HideInInspector]public SpriteRenderer mRender;//������Ԥ���õ�

    //=======================================

    TreeFocus focus;

    public List<TreePart> GetParts(TreeFocus shelf)
    {
        focus = shelf;
        List<TreePart> parts = new List<TreePart>();
        parts.Add(new TreePart(EFgrNode.balance, transform.GetChild(0).GetComponent<ActionBalance>()));
        parts.Add(new TreePart(EFgrNode.can_do, transform.GetChild(1).GetComponent<ActionCanDo>()));
        return parts;
    }

    public bool HearOfDown(InfoSeal seal)
    {
        if (seal == EFgrMsg.kill_render)
        {
            Destroy(mRender);
        }
        return true;
    }

    public object RespondRequestFromDown(InfoSeal seal)
    {
        return true;
    }

    public void SelfReady()
    {
        mRender = transform.GetComponent<SpriteRenderer>();
    }
}
