using UnityEngine;
using UnityEngine.UI;

public enum EKindUiOffset { rect, scale, ui_pos, opacity }//ƫ�����ʲô

public class ToolOffsetUiLinear : LinearOffset
{//��������UI�ֲ�Ч���ģ����ܵ���ʹ��
 //��ʵҲ�����õ��������棬���ǻḺ���أ��ͽ���ֻר����������UI
 //��ѧ�ϵİ��������Aֵ��Bֵ��״̬�����岻��(��ÿ�����嶼��Ҫר�����ã�����֧��)

    public struct InfoTarget
    {
        public RectTransform rect;
        public Text text;
        public Image image;

        public InfoTarget(RectTransform rect, Text text = null, Image image = null)
        {
            this.rect = rect;
            this.text = text; 
            this.image = image;
        }
    }

    public void MakeReady(InfoTarget pawn, IfoOffset refer, EKindUiOffset kind, bool ignoreWarn = true)
    {
        SetRefer(refer, ignoreWarn);

        mPawn = pawn;//��������

        mKind = kind;//������ʽ

        AssembleRespond();
    }

    public void SuResetBy(IfoOffset refer, bool ignoreWarn = true)
    {
        SetRefer(refer, ignoreWarn);
    }

    //===================================================

    InfoTarget mPawn;
    EKindUiOffset mKind;

    void AssembleRespond()
    {
        if (mKind == EKindUiOffset.rect)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.rect.sizeDelta.x;
                else return mPawn.rect.sizeDelta.y;
            };
            ApplyValue = (result) =>
            {
                mPawn.rect.sizeDelta = result;
            };
        }
        else if (mKind == EKindUiOffset.scale)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.rect.localScale.x;
                else return mPawn.rect.localScale.y;
            };
            ApplyValue = (result) =>
            {
                mPawn.rect.localScale = result;
            };
        }
        else if (mKind == EKindUiOffset.ui_pos)
        {
            GetFitValue = (compare) =>
            {
                if (compare) return mPawn.rect.localPosition.x;
                else return mPawn.rect.localPosition.y;
            };
            ApplyValue = (result) =>
            {
                mPawn.rect.localPosition = result;
            };
        }
        else if (mKind == EKindUiOffset.opacity)
        {
            GetFitValue = (compare) =>
            {
                float value = mPawn.text != null ? mPawn.text.color.a : mPawn.image.color.a;
                return value;
            };
            ApplyValue = (result) =>
            {
                Color now;
                if (mPawn.text != null)
                {
                    now = mPawn.text.color;
                    now.a = result.x;
                    mPawn.text.color = now;
                }
                else if (mPawn.image != null)
                {
                    now = mPawn.image.color;
                    now.a = result.x;
                    mPawn.image.color = now;
                }
            };
        }
        else
        {
            GetFitValue = (compare) =>
            {
                Debug.Log("��֧��ʶ��");
                return 0;
            };
            ApplyValue = (result) =>
            {
                Debug.Log("��֧��ʶ��");
            };
        }
    }
}

