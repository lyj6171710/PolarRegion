using UnityEngine;
using UnityEngine.UI;

public enum EKindUiOffset { rect, scale, ui_pos, opacity }//偏差的是什么

public class ToolOffsetUiLinear : LinearOffset
{//帮助构建UI局部效果的，不能单独使用
 //其实也可以用到其它方面，但是会负担重，就建议只专门用来服务UI
 //数学上的帮助，相对A值到B值的状态，意义不限(但每种意义都需要专门设置，才能支持)

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

        mPawn = pawn;//操作对象

        mKind = kind;//操作方式

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
                Debug.Log("不支持识别");
                return 0;
            };
            ApplyValue = (result) =>
            {
                Debug.Log("不支持识别");
            };
        }
    }
}

