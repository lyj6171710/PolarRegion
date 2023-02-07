using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EMV_Mode { drag, edge }

public class MouseView : MonoBehaviour {//鼠标操纵视野，借助有控制鼠标视野的另有组件
    
    [Range(1f,10f)]
    public float mSpeedTransfer=10f;
    public EMV_Mode mMode;

    bool mCanTransfer;
    bool mCanBroaden;
    Camera mCamera;//这里的new是为隐藏父类已有的成员camera

    Vector2 mAimNeed;
    Vector2 mAimLast;
    float mOrthogStart;
    const float cEdgeRange = 20;

    void Awake()
    {
        mCamera = GetComponent<Camera>();
        mCanTransfer = true;
        mCanBroaden = true;
        mOrthogStart = mCamera.orthographicSize;
    }

    void Update () {
        if (mCanTransfer)
        {
            switch (mMode)
            {
                case EMV_Mode.drag:TryDrag(); break;
                case EMV_Mode.edge:TryEdge(); break;
                default:break;
            }
            CameraAim.It.SetFollowToPos(mAimNeed.x, mAimNeed.y);
        }

        if(mCanBroaden) TryBroaden();

        //TryFollow();//暂时不启用追踪单个物体的功能
    }

    void TryDrag()
    {//鼠标拖拽时，视野随之拖拽(看似场景被拖拽)
        if (Input.GetMouseButton(1))
        {
            mAimNeed.x = transform.position.x - Input.GetAxis("Mouse X") * mSpeedTransfer;//变化转移
            mAimNeed.y = transform.position.y - Input.GetAxis("Mouse Y") * mSpeedTransfer;
        }
    }

    void TryEdge()
    {//鼠标在左侧时，视野往左移动，右侧时往右移动
        float multiple = 50;
        Vector3 viewPos = SceneViewL.It.SuPosRatioInView(UnifiedCursor.It.meCursorPos);

        if (viewPos.x > 0.95f)//如果鼠标位置在右侧
            mAimNeed.x = transform.position.x + mSpeedTransfer * multiple * Time.deltaTime;//就向右移动
        else if (viewPos.x < 0.05f)
            mAimNeed.x = transform.position.x - mSpeedTransfer * multiple * Time.deltaTime;
        else
            mAimNeed.x = transform.position.x;

        if (viewPos.y > 0.95f)
            mAimNeed.y = transform.position.y + mSpeedTransfer * multiple * Time.deltaTime;
        else if (viewPos.y < 0.05f)
            mAimNeed.y = transform.position.y - mSpeedTransfer * multiple * Time.deltaTime;
        else
            mAimNeed.y = transform.position.y;
    }

    void TryFollow()
    {//对场景中物体用鼠标左键点击，将对该物体进行追踪
        if (UnifiedInput.It.meTapConfirm())//只能在点击后执行一次检测，避免连续检测鼠标位置而又取消掉了跟踪的情况
        {
            GameObject hit = CoordUse.SuCatchGameObject(UnifiedCursor.It.meCursorPos);
            if (hit)//获得鼠标正点住的游戏物体(需要有collider组件)
                CameraAim.It.SetFollowTarget(hit.transform);
            else//点击空白，则取消追踪
                CameraAim.It.StopFollow();
        }
    }

     void TryBroaden()
    {//用鼠标滚轮，控制视野大小
        SceneViewL.It.SuOffsetViewContentSize(-Input.GetAxis("Mouse ScrollWheel"));
    }

    //外界可用==========================================

    public void MakeReset()//回归视野到零点
    {
        CameraAim.It.StopFollow();
        transform.position = new Vector3(0, 0, -5);
        CameraAim.It.SetFollowToPos(0, 0);
    }

    public void ViewTransferTo(Vector3 aim_pos, bool moment = false, float view_speed = 1)
    {
        CameraAim.It.SetFollowToPos(aim_pos.x, aim_pos.y);
        if (!moment){
            StartCoroutine(ViewSpeedChange(view_speed, aim_pos));
        }
        else{
            transform.position = new Vector3(aim_pos.x, aim_pos.y, transform.position.z);
        } 
    }
    
    public void ViewLimitIn(Vector2 high,Vector2 low)
    {
        CameraAim.It.mViewLimitDp = true;
        CameraAim.It.mLimitDlDp = low;
        CameraAim.It.mLimitUrDp = high;
    }

    public void LockView(bool sure)//视野变化不被玩家操控
    {
        if (sure){
            mCanBroaden = false;
            mCanTransfer = false;
        }
        else {
            mCanTransfer = true;
            mCanBroaden = true;
        }
    }

    public void ResumeValue()
    {
        mCamera.orthographicSize = mOrthogStart;
    }

    //内部工具=============================

    IEnumerator ViewSpeedChange(float percent_to, float second)
    {
        float gap = MakeGap(percent_to);
        yield return new WaitForSeconds(second);
        CameraAim.It.mSmoothDp -= gap;
    }
    
    IEnumerator ViewSpeedChange(float viewSpeed, Vector3 aimPos)//视野移动到指定位置后，才会恢复移速
    {
        float gap = MakeGap(viewSpeed);
        while (Vector3.Distance(aimPos, transform.position) > 6){
            CameraAim.It.SetFollowToPos(aimPos.x, aimPos.y);
            if (!CameraAim.It.FollowAsk) break;//被打断
            yield return null;
        }
        CameraAim.It.mSmoothDp -= gap;
    }

     float MakeGap(float view_speed)//骤变视野移动速度
    {
        view_speed = Mathf.Clamp(view_speed, 0.1f, 10);
        float last = CameraAim.It.mSmoothDp;
        float need = last * view_speed;
        CameraAim.It.mSmoothDp = need;
        return need - last;
    }

    //=============================================

    static MouseView it;
    public static MouseView It
    { get { if (!it) it = GameObject.Find("Main Camera").GetComponent<MouseView>(); return it; } }


}
