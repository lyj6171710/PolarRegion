using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//基本机制是：用物体列表中的每一个物体来播放每一个音乐，
//物体列表所含物体数量，由从开始到当前时刻有过的，同一时刻正播放有音乐的最大数量

public class AudioPlace : MonoBehaviour,ISwitchScene//待办：改过但未仔细检查过，可能会出问题，需要修正
{
    //--------------------------------
    
    //主音量以及所有声效的共通控制

    public float mVolumeScaleWant = 1;//目标值，范围要求0到1
    public void AdjustVolume(float degree)//也可以直接调公开的声量属性，这里只是提供另一种方法途径
    {
        mVolumeScaleWant = degree;
    }

    float mVscale;//应用值

    //外界可用=============================================

    //主音量---------------------------------------

    float vscale_now = 1;//主音乐音量的伸缩，不要直接在这改

    AudioSource mSelfEmit { get; set; }//系统播放音乐的组件
    Coroutine fade;
    Coroutine back;

    AudioPlayer.InfoSound mUse;
    AudioPlayer.InfoSound mLast;

    public void PlayBgm(AudioClip clip, float volume = 1, float tone = 1)
    {//播放背景音，该类音乐一般很长，且循环，且同时只能有一个

        if (back != null) StopCoroutine(back);
        RevokeFade();

        if (!mSelfEmit)
        {
            mSelfEmit = gameObject.AddComponent<AudioSource>();
            mSelfEmit.loop = true;
        }
        if (clip)
        {
            if (mUse.clip == clip && mUse.tone == tone)
            {//同一首背景音，则保持继续
                mSelfEmit.volume = volume;
                mUse.volume = volume;
            }
            else
            {//不是同一首，直接替换
                mLast = mUse;
                mUse.clip = clip;
                mUse.volume = volume;
                mUse.tone = tone;

                mSelfEmit.Stop();
                PutIntoBgm(mUse);//将所给予给的参数交付给播放器
                mSelfEmit.Play();
            }
        }
        else
        {
            mSelfEmit.Stop();
        }
    }

    public void PauseBgm()
    {
        mSelfEmit.Pause();
    }

    public void ContinueBgm()
    {
        mSelfEmit.Play();
    }

    public void FadeBgm()
    {
        fade = StartCoroutine(FadeVolume());
    }

    public void AdjustBgm(float volumeScale)
    {
        vscale_now = volumeScale;
        if (mSelfEmit) mSelfEmit.volume = mUse.volume * vscale_now;
    }

    public void InsertBgm(AudioClip clip, float keepTime, float volume = 1, float tone = 1)
    {//插入播放一段背景音乐后，继续并重新播放插入前播放的音乐
        if (!mSelfEmit)
            mSelfEmit = GetComponent<AudioSource>();
        PlayBgm(clip, volume, tone);

        back = StartCoroutine(WaitDeal.DelayCall(() =>
         {
             PlayBgm(mLast.clip, mLast.volume, mLast.tone);
         }, keepTime));
    }

    void PutIntoBgm(AudioPlayer.InfoSound need)
    {
        mSelfEmit.clip = need.clip;
        mSelfEmit.volume = need.volume * vscale_now;
        mSelfEmit.pitch = need.tone;
    }

    IEnumerator FadeVolume()
    {
        float gap = mSelfEmit.volume * 0.05f;
        for (; mSelfEmit.volume > 0;)
        {
            mSelfEmit.volume -= gap;
            yield return new WaitForSeconds(0.25f);
        }
    }

    void RevokeFade()
    {
        if (fade != null) StopCoroutine(fade);
        AdjustBgm(vscale_now);//通过该函数可以直接回复到应有状态
    }

    //各声效--------------------------------------

    List<AudioPlayer> mPlayers = new List<AudioPlayer>();//对各音乐物体所含音乐组件的引用列表

    public int PlaySound(AudioPlayer.InfoSound refer)
    {//播放一遍指定音响，默认情况是2d音乐
        if (!refer.CheckValid()) return -1;//非法音频参数
        int i = 0;//顺便返回音乐所在的列表项
        bool found_idle = false;
        while (i < mPlayers.Count)
        {
            if (!mPlayers[i].isPlaying)//发现有音乐物体没有播放有音乐，则使用它来播放
            {
                found_idle = true;//标记发现有
                break;
            }
            i++ ;
        }
        if (!found_idle)
        {//如果没有空闲的音乐物体
            AudioPlayer player = CreateOnePlayer();
            mPlayers.Add(player);
            i = mPlayers.Count - 1;
        }
        mPlayers[i].SetAndUseNer(refer, false, vscale_now);
        mPlayers[i].Go();
        return i;
    }

    public void PlaySoundNormal(AudioClip clip)
    {
        PlaySound(new AudioPlayer.InfoSound(clip));//这个构造函数，保证了同时会给予各属性默认值
    }

    public void PlaySoundNormal(AudioClip clip, Action whenEnd)
    {
        AudioPlayer.InfoSound refer = new AudioPlayer.InfoSound(clip);
        refer.whenEnd = whenEnd;
        PlaySound(refer);
    }

    public void PlaySoundStay(AudioPlayer.InfoSound refer, bool onoff)
    {//让音乐物体列表中有且只有一个音乐物体在循环播放指定音响
     //参数on为开关，对所指定音响，指定是否播放的开关
     //只适合某一物体完全控制某一音响，不过可以连续调用该函数，最初设计是用来解决脚步声问题
        int target =0;
        bool haveFound = false;
        bool haveOn = false;//是否已经播放有(已有且正播放)
        for (int i = 0; i < mPlayers.Count; ++i)
        {//先看当前有没有，含所需播放音乐的音乐物体，准备排除或利用
            if (mPlayers[i].meInfo.clip == refer.clip && mPlayers[i].meInfo.tone == refer.tone)
            {//是否同个不仅要取材相同，还要音调相同
                if (!haveFound)
                {
                    target = i;
                    //会针对第一次查到的音乐物体
                    haveFound = true;
                    //已有含所需播放音乐的音乐物体
                    //一旦已有则该函数的命令转向为利用它，不再进行其它活动
                }

                mPlayers[i].SetAndUseNer(refer, true, vscale_now);//默认就让它继续,以下判断其它情况

                if (mPlayers[i].isPlaying) 
                {//正播放有，则直接使用它
                    if (!onoff)//一旦要求关闭，任何正有播放都得停止
                        mPlayers[i].Pause();
                    else
                    {
                        if (haveOn)//如果要求播放但已有播放，也得停止播放，避免重复
                            mPlayers[i].Pause();
                        else
                        {
                            mPlayers[i].Go();
                            haveOn = true;//执行到这前，可能还认为没播放有，这里有了，所以要标志出来
                        }
                    }
                }
                else
                {//发现已有，但没有播放
                    if (onoff)
                    {
                        if (!haveOn)//还没有正播放有的
                        {
                            mPlayers[i].Go();
                            haveOn = true;//将不再启动其它音乐物体对同个音响的播放
                        }
                    }
                    //如果要求关闭，本来就没有播放则已经顺应要求
                }
            }
        }
        if (!haveFound)
        {//没有现成的，则要么使用空闲者，要么创建新的
            mPlayers[PlaySound(refer)].KeepOn();
        }
    }
    
    public void PlaySound3d(AudioPlayer.InfoSound refer, Transform area)
    {//所谓3d,会随玩家离播放源的位置状态，改变所播放音乐的左右偏向与音量，
     //玩家根据自己行为对声响属性的变换，也将有能力判断出发声源相对自己角色的方向
        int load = PlaySound(refer);
        mPlayers[load].To3dNer(area);//不断刷新属性
        mPlayers[load].Go();
    }

    public AudioPlayer PlaySound3dStay(AudioPlayer.InfoSound refer, Transform area)
    {//播放会自我循环的音响，何时开停由外界控制
     //外界不能连续调用该函数，适合不同物体分别循环播放同一种音效
        
        int load = PlaySound(refer);
        AudioPlayer player = mPlayers[load];
        player.AdjustSoundNer(-1, true);//负数代表保持不变，true代表使循环
        player.To3dNer(area);//3d化
        player.transform.SetParent(area);//从属到申请者上
        mPlayers.Remove(player);//从该系统剔除出去，由申请者自己掌管
        return player;

    }

    AudioPlayer CreateOnePlayer()
    {
        GameObject example = new GameObject("player");
        example.transform.SetParent(transform);
        AudioPlayer player = example.AddComponent<AudioPlayer>();
        return player;
    }

    //内部机制==============================================

    void AdjustVolume()
    {
        AdjustBgm(mVolumeScaleWant);//这里只是调整主背景音
        foreach (AudioPlayer tmp in mPlayers)
            tmp.AdjustSoundNer(mVolumeScaleWant);
    }

    //架构需要==========================================

    public static AudioPlace It;

    public void WhenAwake()
    {
        It = this;
    }

    public void WhenSwitchScene()
    {

    }

    void Start()
    {
        AdjustVolume();
    }

    void Update()
    {
        if (mVolumeScaleWant != mVscale)
        {
            AdjustVolume();
            mVscale = mVolumeScaleWant;
        }
    }
}