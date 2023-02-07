using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioPlayer : MonoBehaviour {
    //前提是一个在播放开始到播放完期间，不会受到干扰的情况下，可使用

    [System.Serializable]
    public struct InfoSound
    {
        public string call;
        public AudioClip clip;
        public float volume;//音量基准，初始化后就不要随意改动了
        public float tone;
        public float bias;
        public Action whenEnd;

        public InfoSound(AudioClip clip)//赋予默认值
        {
            this.clip = clip;
            volume = 1;
            tone = 1;
            bias = 0;
            call = "";
            whenEnd = null;
        }

        public bool CheckValid()
        {
            return clip != null;
        }
    }

    //----------------------------------------

    InfoSound mInfo;
    AudioSource mEmit;

    float mVScale = 1;//音量放缩，建议最后交付时使用，相对于全局
    float mSpeed = 1;//速度越大，音量越大，相对于个体

    bool mIs3D;//是否是3d音效
    Transform mRefer3D;

    bool mInLoop;

    float mRestTime;
    bool mHaveEnd;

    void Awake()
    {
        mEmit=gameObject.AddComponent<AudioSource>();
        mEmit.playOnAwake = false;
    }

    void Update()
    {
        if (!mHaveEnd && mInfo.whenEnd != null) 
        {
            if (isPlaying)
            {
                if (mRestTime > 0)
                {
                    mRestTime -= Time.deltaTime;
                }
            }
            else if (mRestTime <= Time.deltaTime)//不要放play期间去判断，而且这里要有一个时间单位的误差，避免极端情况
            {
                mInfo.whenEnd();
                mHaveEnd = true;
            }
        }
    }

    //内外机制=================================

    public void SetAndUseNer(InfoSound refer, bool loop, float vscale_cur)//基准数据
    {
        mInfo = refer;
        mInLoop = loop;
        AdjustSoundNer(vscale_cur, loop);
        PutInto();

        mRestTime = refer.clip.length;
        mHaveEnd = false;
    }

    public void AdjustSoundNer(float vscale_cur, bool loop = false)
    {
        if (vscale_cur >= 0) mVScale = vscale_cur;//负数时保持原有缩放
        mInLoop = loop;
        PutInto();
    }

    public void To3dNer(Transform area)
    {
        mIs3D = true;
        mRefer3D = area;
    }
    
    //外界可用=================================

    public bool isPlaying { get { return mEmit.isPlaying; } }
    public InfoSound meInfo { get { return mInfo; } }

    public void Pause() {
        mEmit.Pause();
        StopAllCoroutines();
    }

    public void Go()
    {
        if (!mEmit.isPlaying) mEmit.Play();
        if (mIs3D) StartCoroutine(IeRefreshAttr());
    }

    public void KeepOn()
    {
        AdjustSoundNer(-1, true);
        Go();
    }

    public void SetSpeed(float speed)
    {
        mSpeed = Mathf.Clamp(speed, 0.1f, 2);
    }

    //内部工具=================================

    void PutInto()
    {
        PutIntoEmit(mInfo);
    }

    void PutIntoEmit(InfoSound refer)
    {
        mEmit.clip = refer.clip;
        mEmit.volume = refer.volume * mVScale * mSpeed;
        mEmit.pitch = refer.tone;
        mEmit.panStereo = refer.bias;
        mEmit.loop = mInLoop;
    }

    IEnumerator IeRefreshAttr()
    {//不断检测并根据当前状态刷新属性,主要针对音量和声道
        Transform here;
        if (Camera.main)
            here = Camera.main.transform;
        else
            here = null;

        bool firstWait = true;
        while (true)
        {
            if (!isPlaying && firstWait)//执行就绪
            {
                yield return null;
                continue;
            }
            else
            {
                firstWait = false;

                float tmpVol = mInfo.volume;//中介
                float bias = mInfo.bias;//声道
                Vector3 tmpGap;

                if (mRefer3D && here)
                {
                    tmpGap = here.position - mRefer3D.position;
                    //重新检测位置差距状态
                    float distance = new Vector2(tmpGap.x, tmpGap.y).magnitude;
                    float persontage = Mathf.Clamp(1 - distance * 0.1f, 0, 1);
                    tmpVol = mInfo.volume * persontage;//volume是原始参照
                                                       //发声源与视野中心每拉远1个单位距离，音量大小将以原来音量的百分之10下调,直到完全没有声响
                    bias = Mathf.Clamp(tmpGap.x * (-0.1f), -1, 1);
                    //发生源与视野中心在x方向上每向右拉远1个单位距离，声道偏向将往左变动0.1，直到无法再往相应方向变动
                }
                else
                    tmpVol = tmpVol - 0.02f;//递弱

                InfoSound refer = new InfoSound(mInfo.clip);
                refer.volume = tmpVol;
                refer.tone = mInfo.tone;
                refer.bias = bias;
                PutIntoEmit(refer);
                if (isPlaying)//如果还在播放
                    yield return null;//每帧重新解读并刷新属性
                else
                    break;//不加break的话，后来音效占据使用该播放器实体时会被干涉
            }
        }
    }

}
