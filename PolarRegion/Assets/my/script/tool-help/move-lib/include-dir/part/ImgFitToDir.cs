using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgFitToDir : MonoBehaviour, IDirGet
{//当图片本身就朝右时，这里就能让图片朝向给定的方向
 //会相对场景变化方向，无视本地空间，如果要考虑，外界可以自己加入本地因素后，再将结果值传递给该组件
 //该组件承包该物体的旋转状态，外界不要再自己修改影响旋转的因素

    public Vector2 meDirAim;//凸显目标性，不是瞬间适应的
    public float meAngleAim;

    public float meAngleNow { get { return mCurRightTo; } }

    public void SuFitInstantly()//无渐变效果，瞬间到达指定方向
    {
        mCurRightTo = GetDirNeed();//直达目标值
        FitToNewDir(mCurRightTo);
    }

    public void SuAutoFit(bool onoff)//自动跟随所设置的目标值
    {
        if (!mHaveReady) return;
        if (onoff)
        {
            mCurRightTo = MathAngle.GetAngle(transform.right) + mAngleSelf;//获取并基于当前朝向
            mInUse = true;
        }
        else
            mInUse = false;
    }

    //====================================

    float mAngleSelf;//如果图像内容指向不是朝右的，请描述该图像指向，相对正右的逆时针度数
    //该组件的目标是，使得图像内含的正向能顺应所指定的方向
    bool mHaveReady;
    EAngle mFormUse;

    bool mStartMirror;
    int mStartMirrorSelf;
    bool mMirrorIgnore;//考虑镜像的影响会增大性能负担，因此要可以选择性考虑

    float mCurRightTo;//逻辑上，当前图像内容指向相对场右的方向（场右：场景坐标系右轴的指向）
    bool mInUse;//渐变效果

    public void MakeReady(float angleSelf, EAngle form)
    {
        if (mHaveReady) return;

        mFormUse = form;
        mAngleSelf = angleSelf;

        mCurRightTo = mAngleSelf;//图像右侧当前指向哪里

        ConsiderMirror();

        mHaveReady = true;
    }

    void ConsiderMirror()
    {//不一定一开始就会考虑，外界定下该组件所挂载物体的transform状态后再调用?
     //不影响的，考虑镜像，仅仅是不再受镜像影响，给了方向，我就能给你转动过去
        mMirrorIgnore = false;
        mStartMirror = GbjAssist.GetSumScaleWhenParent(transform).x > 0 ? true : false;
        mStartMirrorSelf = transform.localScale.x > 0 ? 1 : -1;
    }

    void LateUpdate()
    {
        if (!mHaveReady) return;

        if (mInUse)
        {
            float needDir = GetDirNeed();
            mCurRightTo = MathNum.Lerp(mCurRightTo, needDir, 0.25f);
            FitToNewDir(mCurRightTo);
        }
        else
        {
            SuFitInstantly();//及时应用，不管外界哪里有变动
        }

    }

    void FitToNewDir(float angleNeed)
    {
        SetRightTo(MathAngle.AngleToVector(angleNeed - mAngleSelf));
        //图像右侧相对场右方向 = 右轴相对场右的方向 + 源图像相对场右的方向

        GetRidOfMirror();
    }

    void GetRidOfMirror()
    {
        //必需相对场景，维持原来的镜像状态
        //需要每时候都执行，因为外界随时都可能改动了量值，牵连就改变了符号
        if (mMirrorIgnore) return;
        Vector3 origin = transform.localScale;
        if (mStartMirror != GbjAssist.GetSumScaleWhenParent(transform).x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(origin.x) * -1 * mStartMirrorSelf, origin.y, origin.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(origin.x) * mStartMirrorSelf, origin.y, origin.z);
        }
        //父物体镜像化后，子物体想要回到原有状态，需要自身反镜像的同时，旋转角度变负数
        //旋转角度变负数，可以让图像右侧指向原来所指的方向（如果能维持右侧方向，那么不需要手动置负数）
    }

    float GetDirNeed()//使用度数表达方向，更易理解，也会利于实现插值
    {
        if (mFormUse == EAngle.vector)
            return MathAngle.GetAngle(meDirAim);
        else if (mFormUse == EAngle.degree)
            return meAngleAim;
        else 
            return 0;
    }

    void SetRightTo(Vector2 dir)//并没有考虑图像本身的状态，需要已经凝练在参数中
    {
        if (Vector2.Dot(dir, Vector2.left) < 0.9995f) //这种旋转方式，好像不支持正左方向，所以要在转至前停下
            transform.right = dir;//transform.right是相对场景的
                                  //设置时相对场景，但它自身方向可能会被动被改变的
        else
        {
            if (dir.y >= 0)
                transform.right = new Vector2(-1, 0.001f);
            else
                transform.right = new Vector2(-1, -0.001f);
        }
        //使右轴在一个方向上时，还不能保证上轴在右轴的左侧还是右侧（但是unity机制，会让上轴一直在右轴的左侧，这里就不用顾及）
    }

    //============================
    //另外相关知识：
    //localEulerAngles.x：这才是编辑器transform显示栏中的旋转值，而rotation.x是四元数的一个维度
}
