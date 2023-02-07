using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour
{
    //全局的能帮助进行选项选择的部门，外界不同部门需要互斥使用该部门的功能
    //基本功能已经实现，但还是建议使用AssistSelect工具替代，后者更安全易用

    //选择框自己负责搜索与前往可以选择的选项，并作黏附
    //需要外界保持该组件所属物体是打开状态
    //该组件就挂载于选择框上

    //外界可用==============================

    public int meSelectAt { get { return mSelectAt; } }
 
    //一次性赋值============================

    public AudioClip 确定音效;
    public AudioClip 回退音效;
    public AudioClip 移换音效;

    //私用变量==============================
    
    Coroutine openning;
    bool mInOpen;//是否正处于开启状态，具有显示出的选框
    IWait mRespond;//当前是谁发起的选择，它会负责进行选择后的逻辑
    IWaitAdd mRespondExtra;//增持的响应
    string mUser;//类似此次选择的id，可帮助外界区分

    Canvas mSelectIn;//当前在哪个画布下进行的选择
    RectTransform mSelectCur;
    int mSelectAt;//与当前所选选项对应，为其在选项列表中的序数
    List<RectTransform> mSelectHave;//当前所有可选选项
    
    float mSizeScale;
    RectTransform mRectSelf;
    Image mImageSelf;
    List<RectTransform> mSelectCan;//当前可立即移动到的选项，临时的
    
    //内部机制==============================
    
    void Update()
    {
        if (mInOpen)
        {
            if (mSelectCur && mSelectHave.Count > 0) 
            {//需先具有能操作的环境
                ReactShift();//移动反应
                ReactConfirm();
            }
        }
    }

    void ReactShift()//识别用户对选框的移动操作，并做出特定反应
    {
        if (UnifiedCursor.It.meIsSweeping && UnifiedCursor.It.meUiOver != null)
        {
            RectTransform select_want = UnifiedCursor.It.meUiOver.GetComponent<RectTransform>();//鼠标悬浮在的ui是玩家想要点击的ui
            ShiftTo(select_want);
        }
        else
        {
            EToward4 curGoTo = UnifiedInput.It.meGoOneStep();
            if (curGoTo == EToward4.left) 
            {
                ProReactShift_(EToward4.left,
                    (have, cur) => { if (have.x < cur.x) return true; else return false; },
                    (can, cur) => { if (can.y < cur.y - mSelectCur.rect.height / 10f || can.y > cur.y + mSelectCur.rect.height / 10f) return true; else return false; },
                    (can, nearest) => { if (can.x > nearest.x) return true; else return false; }
                    );
            }
            else if (curGoTo == EToward4.right)
            {
                ProReactShift_(EToward4.right,
                    (have, cur) => { if (have.x > cur.x) return true; else return false; },
                    (can, cur) => { if (can.y < cur.y - mSelectCur.rect.height / 10f || can.y > cur.y + mSelectCur.rect.height / 10f) return true; else return false; },
                    (can, nearest) => { if (can.x < nearest.x) return true; else return false; }
                    );
            }
            else if (curGoTo == EToward4.up)
            {
                ProReactShift_(EToward4.up,
                      (have, cur) => { if (have.y > cur.y) return true; else return false; },
                      (can, cur) => { if (can.x < cur.x - mSelectCur.rect.width / 5f || can.x > cur.x + mSelectCur.rect.width / 5f) return true; else return false; },
                      (can, nearest) => { if (can.y < nearest.y) return true; else return false; }
                      );
            }
            else if (curGoTo == EToward4.down)
            {
                ProReactShift_(EToward4.down,
                      (have, cur) => { if (have.y < cur.y) return true; else return false; },
                      (can, cur) => { if (can.x < cur.x - mSelectCur.rect.width / 5f || can.x > cur.x + mSelectCur.rect.width / 5f) return true; else return false; },
                      (can, nearest) => { if (can.y > nearest.y) return true; else return false; }
                      );
            }
        }
    }

    void ProReactShift_(EToward4 to, Func<Vector2, Vector2, bool> filter_base_, Func<Vector2, Vector2, bool> filter_line_, Func<Vector2, Vector2, bool> filter_first_)
    {
        for (int i = 0; i < mSelectHave.Count; i++)
        {//收集所有处于某一边的可选选项
            if (filter_base_(mSelectHave[i].position,mSelectCur.position))
                mSelectCan.Add(mSelectHave[i]);
        }

        for (int i = 0; i < mSelectCan.Count; i++)
        {//进一步筛选可选选项，缩减可选选项数量
            if (filter_line_(mSelectCan[i].position, mSelectCur.position))
            {//需要在某线上
                mSelectCan.RemoveAt(i);
                i--;
            }
        }

        if (mSelectCan.Count > 0)
        {//看是否还具有可选选项
            RectTransform nearest = mSelectCan[0];//nearest用来记录直到当前所检测到的最近者
            for (int i = 1; i < mSelectCan.Count; i++)
            {
                if (filter_first_(mSelectCan[i].position, nearest.position))
                    nearest = mSelectCan[i];
            }
            Debug.Log(nearest.gameObject.name);
            ShiftTo(nearest);
        }
        else if (mRespondExtra != null)
            mRespondExtra.WhenBlock(to, mUser);//无可迁移到的选项时
    }
    
    void ReactConfirm()//识别用户对当前所选选项的操作，并做出反映
    {
        if (UnifiedInput.It.meTapConfirm())//确定操作时
        {
            mRespond.WhenSure(GetIndex(mSelectCur), mUser);
            //要注意外界可能会在这个顺带执行的外界流程中，突然关闭选择器，
            //导致这里的某些后续流程失去原来应有的参数，因此这里后续就不要再有对选择器的操作了
        }
        else if (UnifiedInput.It.meWhenBack())//返回操作时
        {
            mRespond.WhenBack(mUser);
        }
    }

    //内部接口===================================

    void ShiftTo(RectTransform target)//移动到另一选项的数据处理
    {
        mSelectCur = target;//刷新
        mSelectCan.RemoveRange(0, mSelectCan.Count);//可选选项表是在每次移动选框时临时重建的，因此需要每次移动后重置
        
        mRectSelf.sizeDelta = new Vector2(mSelectCur.rect.width , mSelectCur.rect.height ) * mSizeScale;//根据所处选项的大小决定选项框的大小
        mRectSelf.position = mSelectCur.position;//根据所处选项的位置决定选项框的位置

        mSelectAt = GetIndex(target);
        if (mRespondExtra != null) mRespondExtra.WhenShiftTo(mSelectAt, mUser);
    }

    void MakeShow(bool onoff)
    {
        Color tmp = mImageSelf.color;
        if (onoff)
            mImageSelf.color = new Color(tmp.r, tmp.g, tmp.b, 1f);
        else
            mImageSelf.color = new Color(tmp.r, tmp.g, tmp.b, 0);
    }
    
    int GetIndex(RectTransform option)
    {
        for (int i = 0; i < mSelectHave.Count; i++)
        {
            if (mSelectHave[i] == option)
                return i;
        }
        return -1;
    }

    //组合可用========================================

    public MenuSelector SuUseFor(IWait wait, Transform parent, int optionNum, int start = 0, string id = "")//投入使用
    {
        Func<List<RectTransform>> GetOptions = () =>
          {
              List<RectTransform> options = new List<RectTransform>();
              for (int i = 0; i < parent.childCount; i++)
              {
                  if (options.Count < optionNum)
                  {
                      GameObject child = parent.GetChild(i).gameObject;
                      options.Add(child.GetComponent<RectTransform>());
                  }
                  else
                      break;
              }
              return options;
          };
        return ProUseFor(wait, GetOptions, start, id);
    }

    public MenuSelector SuUseFor(IWait wait, List<GameObject> selects, int start = 0, string id = "")
    {
        Func<List<RectTransform>> GetOptions = () =>
        {
            List<RectTransform> options = new List<RectTransform>();
            for (int i = 0; i < selects.Count; i++)
                options.Add(selects[i].GetComponent<RectTransform>());
            return options;
        };
        return ProUseFor(wait, GetOptions, start, id);
    }

    MenuSelector ProUseFor(IWait wait, Func<List<RectTransform>> GetOptions, int start = 0, string id = "")//投入使用
    {
        SuClose();//同时会撤除原有选择对象（打断）

        mSelectCan = new List<RectTransform>();
        List<RectTransform> options = GetOptions();
        mSelectIn = GbjAssist.GetCompInUpper<Canvas>(options[0].parent);

        if (mSelectIn == null) //无效的选项列表，则不做选择
        {
            Debug.Log("可选项不符合规格");
        }
        else
        {
            openning = StartCoroutine(WaitDeal.DelayCall(() =>
            {
                //延迟期间，外界可能撤除了选项

                if (options != null && options.Count > 0)
                {
                    //有效时，显化选框并可以进行选择了
                    mRespond = wait;
                    mUser = id;

                    foreach (RectTransform option in options) option.gameObject.tag = "select";//要求可选的，一定就是需要筛选出的
                    UnifiedCursor.It.SuListenOver(mSelectIn.gameObject, "select");//去识别带有select标签的物体
                    mSelectHave = options;
                    ShiftTo(options[start]);

                    MakeShow(true);
                    mInOpen = true;//表示处于开启状态了
                }
                else
                    SuClose();

            }, 2));//延迟调用，因为选项的布局情况可能受外物支配，就需要先等外物确定好位置，此时选框的位置才会正确
        }
        return this;//外界可以继续调用所含接口进行进一步的设置
    }

    public void SuSetScale(float ratio)
    {
        mSizeScale = Mathf.Clamp(ratio, 0.05f, 2f);
        if (mSelectCur) ShiftTo(mSelectCur);//立即适应上去
    }

    public void SuAddRespond(IWaitAdd wait)
    {
        mRespondExtra = wait;
    }

    public void SuClose()//外界可以手动关闭选择，有时也必需采取手动关闭
    {//当启动对另一个选项列表的选择时，会自动关闭对当前选择列表的选择
        if (mInOpen)
        {
            foreach (RectTransform option in mSelectHave)
                option.gameObject.tag = "Untagged";

            mInOpen = false;
        }

        if (openning != null) StopCoroutine(openning);
        
        mUser = "";
        
        MakeShow(false);
    } //不建议该组件在某些方面某些时候自动负责选择的关闭，容易出问题

    //架构需要===============================

    public static MenuSelector It;

    void Awake()
    {
        It = this;
        mSizeScale = 1;
        mImageSelf = GetComponent<Image>();
        mImageSelf.enabled = true;
        mRectSelf = GetComponent<RectTransform>();
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = GlobalConfig.prior_ui + 1;//让选框优先显示
        CanvasScaler scaler = transform.GetComponentInParent<CanvasScaler>();
        MakeShow(false);
    }
    
    public interface IWait
    {
        void WhenSure(int index,string user);
        void WhenBack(string user);
    }

    public interface IWaitAdd
    {
        void WhenBlock(EToward4 to, string user);
        void WhenShiftTo(int index, string user);
    }
}
