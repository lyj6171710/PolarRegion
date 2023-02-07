using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundRefer {
    public string call;
    public AudioClip clip;
    public float volume=1;
    public float tone=1;
}

public class SoundPick : MonoBehaviour {//将音效集中在一个地方
    //依赖audio_place组件

     static SoundPick single;//该组件
    public static SoundPick Single
    {
        get
        {
            if (!single)
                single = GameObject.Find("Audio-place").GetComponent<SoundPick>();
            return single;
        }
    }
    
    public float volume_scale=1;//目标值，范围要求0到1
     float vscale;//实际值
    public float Volume_scale//监听变化
    {
        get { return vscale; }
        set {
            if (value != vscale)
                adjust_volume_();
            vscale = value;
        }
    }
    
    [Space(2.5f)] public SoundRefer[] sounds_refer;
     AudioPlace place;

    //===================================

     void Awake()
    {
        if (single != null) Destroy(gameObject);//防止存在多个
        else
        {
            if (Single)//这里催促赋上值，避免他人趁机占用了
                DontDestroyOnLoad(gameObject);//唯一时，长期存续
        }

        place = GetComponent<AudioPlace>();
    }

     void Start()
    {
        adjust_volume_();
    }

     void Update()
    {
        Volume_scale = volume_scale;
    }

    //===============================================

    public void play_sound_3d_(string call, Transform area)
    {//一次性音效
        SoundRefer pick = pick_sound_(call);
        if (pick.clip != null)
            ;// place.play_sound_3d_(pick.clip, area, pick.volume, pick.tone);
        else
            Debug.Log("没有该音响引用");
    }
    public GameObject play_sound_3d_stay_(string call, Transform area)
    {//走路类型的音效
        SoundRefer pick = pick_sound_(call);
        if (pick.clip != null)
            return null;// place.play_sound_3d_stay_(pick.clip, area, pick.volume, pick.tone);
        else{
            Debug.Log("没有该音响引用");
            return null;
        }  
    }

     void adjust_volume_()
    {
        //AudioPlace.Single.adjust_bgm_(volume_scale);//这里只是调整主背景音
        //foreach (AudioPlayer tmp in place.players)
        //    tmp.adjust_sound_(volume_scale);
    }

     SoundRefer pick_sound_(string call)
    {
        foreach (SoundRefer pick in sounds_refer)
        {
            if (pick.call == call) return pick;
        }
        return new SoundRefer();
    }

     SoundRefer pick_sound_(AudioClip refer)
    {
        foreach (SoundRefer pick in sounds_refer)
        {
            if (pick.clip == refer)
                return pick;
        }
        return new SoundRefer();
    }

    //外界可用===============================

    public void adjust_volume_(float degree)
    {
        degree = Mathf.Clamp(degree, 0, 1);
        volume_scale = degree;
    }
}
