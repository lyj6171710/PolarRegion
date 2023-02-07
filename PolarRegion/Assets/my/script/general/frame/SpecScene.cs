using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecScene : MonoBehaviour {//负责一个场景特有的流程逻辑

    //外界可用=================================

    public bool cursorVisible { get { return !mHideCursor; } }//让场景管理鼠标状态
    public bool isPlaying { get { return is_playing; } }

    //子类可用=================================

    public GameObject player_focus_inner { get; set; }//玩家当前聚焦与操作的物体

    //私用变量==================================

    protected LocalFocus focus;

    bool is_playing;//标识游戏是否处于运行状态

    MethodRangeAccess mHideCursorCtrl;
    bool mHideCursor;//逻辑上，光标当前应有的显隐状态
    
    protected virtual void Awake()
    {
        root = this;
        is_playing = true;
        focus = GetComponent<LocalFocus>();
        mHideCursor = false;
        mHideCursorCtrl = new MethodRangeAccess();
    }

    protected virtual void Start() { }

    protected virtual void Update()
    {
        inspect_cursor_state_();
    }

    protected virtual void FixedUpdate() { }

    void inspect_cursor_state_()
    {
        if (mHideCursor){
            if (Cursor.visible)//自带的这个接口可能会受其它软件的影响
                Cursor.visible = false;
        }
        else{
            if (!Cursor.visible)
                Cursor.visible = true;
        }
    }

    //外界可用======================================

    public bool AskCursorHide(MonoBehaviour asker)
    {
        if (mHideCursorCtrl.RequestUse(asker))
        {
            mHideCursor = true;
            return true;
        }
        else
            return false;
    }

    public bool AskCursorAppear(MonoBehaviour asker)
    {
        if (mHideCursorCtrl.LeaveUse(asker))
        {
            mHideCursor = false;
            return true;
        }
        else
            return false;
    }

    //架构需要=================================

    static SpecScene root;
    public static SpecScene Root { get { if (root == null) root = FindObjectOfType<SpecScene>();return root; } }

}
