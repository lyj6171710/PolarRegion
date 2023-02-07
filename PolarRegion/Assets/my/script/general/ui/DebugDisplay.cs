using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour,ISwitchScene
{
    public Transform test;

    //==============================

    public bool meOn;
    public string meTxtMid;
    public string meTxtUp;
    public string meTxtDown;
    public string meTxtLeft;
    public string meTxtRight;
    public Sprite mImgToTipPosDp;

    bool haveReady;
    bool inShow;

    GameObject tip;
    Text TxtCentre;
    GameObject tipTxtUp;
    Text TxtUp;
    GameObject tipTxtDdown;
    Text TxtDown;
    GameObject tipTxtLeft;
    Text TxtLeft;
    GameObject tipTxtRight;
    Text TxtRight;

    AlonePic ImgPos;
    GameObject tipImgPos;
    Int stopTipImgPos;

    UiAlone.Ifo mReferTmp;

    // Start is called before the first frame update
    void MakeReady()
    {
        if (!meOn) return;

        mReferTmp = new UiAlone.Ifo();
        mReferTmp.byWorldOrCanvas = false;
        mReferTmp.byAbsoluteOrPercent = false;

        mReferTmp.posStart = new Vector2(0.5f, 0.5f);
        tip = GText.It.SuForm("点击了", 0, mReferTmp); 
        TxtCentre = tip.GetComponent<Text>();
        
        mReferTmp.posStart = new Vector2(0.5f, 0.9f);
        tipTxtUp = GText.It.SuForm("上", 0, mReferTmp);
        TxtUp = tipTxtUp.GetComponent<Text>();
        
        mReferTmp.posStart = new Vector2(0.5f, 0.1f);
        tipTxtDdown = GText.It.SuForm("下", 0, mReferTmp); 
        TxtDown = tipTxtDdown.GetComponent<Text>();
        
        mReferTmp.posStart = new Vector2(0.1f, 0.5f);
        tipTxtLeft = GText.It.SuForm("左", 0, mReferTmp);
        TxtLeft = tipTxtLeft.GetComponent<Text>();
        
        mReferTmp.posStart = new Vector2(0.9f, 0.5f);
        tipTxtRight = GText.It.SuForm("右", 0, mReferTmp); 
        TxtRight = tipTxtRight.GetComponent<Text>();

        SetTxtShow(true);

        tipImgPos = GPic.It.SuForm(mImgToTipPosDp, 0, mReferTmp, 0.5f);
        ImgPos = tipImgPos.GetComponent<AlonePic>();
        stopTipImgPos = new Int();

        haveReady = true;

        HaveTestStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!meOn)
        {
            if(inShow) SetTxtShow(false);
            return;
        }
        else
        {
            if (!haveReady) MakeReady();

            if (!inShow) SetTxtShow(true);

            TxtCentre.text = meTxtMid;
            TxtUp.text = meTxtUp;
            TxtDown.text = meTxtDown;
            TxtLeft.text = meTxtLeft;
            TxtRight.text = meTxtRight;

            HaveTestUpdate();
        }
    }

    void SetTxtShow(bool onoff)
    {
        TxtCentre.enabled = onoff;
        TxtUp.enabled = onoff;
        TxtDown.enabled = onoff;
        TxtLeft.enabled = onoff;
        TxtRight.enabled = onoff;

        inShow = onoff;
    }

    void HaveTestUpdate()
    {
        //if (UnifiedInput.It.meWhenBack)
        //    want = UnifiedInput.It.meWhenBack.ToString();

        //want = UnifiedInput.It.meIsSweeping.ToString();

        //if (UnifiedInput.It.meIsSweeping)
        //{
        //    Vector3 cur = test.transform.position;
        //    Vector2 offset = OperateToScene.It.meSlideInScene;
        //    test.transform.position += new Vector3(offset.x, offset.y, cur.z);// UnifiedInput.It.meOverOffset;
        //}

    }

    void HaveTestStart()
    {
        

    }


    public void SuShowTmpPosAt(Vector3 pos)//一个位置就是一个新图片
    {
        UiAlone.Ifo refer = new UiAlone.Ifo();
        refer.byWorldOrCanvas = false;
        refer.byAbsoluteOrPercent = false;
        GameObject tmpTipPos = GPic.It.SuForm(mImgToTipPosDp, 0, mReferTmp, 0.5f);
        tmpTipPos.GetComponent<AssistPos>().SuUpdatePosByWorld(pos);
        tmpTipPos.GetComponent<AlonePic>().SuOpen();
        WaitDeal.It.Begin(() => { Destroy(tmpTipPos); }, 0.75f);
    }

    public void SuShowNewestTmpPosAt(Vector3 pos)//每个位置会是同一张图片，以最新给的位置确定该图片所在
    {
        tipImgPos.GetComponent<AssistPos>().SuUpdatePosByWorld(pos);
        ImgPos.SuOpen();
        stopTipImgPos++;
        WaitDeal.It.BeginSafe(() => { ImgPos.SuClose(); }, 0.75f, stopTipImgPos);
    }

    //========================================

    public static DebugDisplay It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {

    }
}
