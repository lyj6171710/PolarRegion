using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordMake : MonoBehaviour
{//该组件全局共享一个，不可复制

    public int meRestTime { get { return (int)mRestTime; } }
    public string meTip { get { return mTip; } }
    public bool meHaveEnd { get { if (!mDo && !mStart) return true; else return false; } }
    public AudioClip meOutput { get { if (meHaveEnd)  return mClip;  else return null; } }

    public string meOutputStr { 
        get {
            if (meHaveEnd)
            {
                string tmpStr = mClipStr;
                mClipStr = null;//取出文件后归零
                return tmpStr;
            }
            else
                return null;
        }
    }

    public static RecordMake It;

    //----------------------------------------

    Info mInfo;

    bool mDo;//进行录音
    bool mStart;//已开始录音
    MonoBehaviour mClient;//当前占用录音功能的部门，如果当前未录音，将可以被另外设备抢占

    string[] devices;

    float mRestTime;
    string mTip;

    AudioClip mClip;
    string mClipStr;

    void Awake()
    {
        It = this;

        devices = Microphone.devices;
        if (devices.Length != 0)
        {
            mTip = "设备有麦克风：" + devices[0];
        }
        else
        {
            mTip = "设备没有麦克风";
        }

        mStart = false;
        mDo = false;
        mClient = null;

        mClipStr = null;
    }

    void Update()
    {
        if (mDo)
        {
            if (!mStart)
            {
                mTip = "录音开始";
                mStart = true;
                mRestTime = mInfo.limit;
                //参数一：设备名字，null为默认设备；参数二：是否循环录制；参数三：录制时间（秒）；参数四：音频率
                mClip = Microphone.Start(devices[0], false, mInfo.limit, mInfo.frequency);
                //暂时不懂系统怎么实现这个录音效果的，左边变量的后续还怎么被系统知道的呢？
            }
            else
            {
                mTip = "正在录音";
                mRestTime -= Time.deltaTime;
                if (mRestTime <= 0)//自动结束
                {
                    EndRecord("时间截止");
                    mInfo.WhenFinish();
                }
            }
        }
        else
        {
            if (mStart)//外界自主结束
            {
                EndRecord("录音提前结束");
            }
            else
            {
                //没开始录音，就什么都不用做
            }
        }
    }

    bool IfCanTakeRecord(MonoBehaviour client)//录音权限
    {
        if (client == null)
            return false;
        else if (!meHaveEnd)
        {
            if (client != mClient)
                return false;
            else
                return true;
        }
        else 
            return true;
    }

    public bool OnoffRecord(MonoBehaviour client, Info refer)//外界开关录音的接口
    {
        if (IfCanTakeRecord(client))
        {
            if (!meHaveEnd)//录音还未结束就被终止
            {
                EndRecord("录音提前结束");
                return false;//结束录音时，返回false
            }
            else
            {
                mClient = client;
                mInfo = refer;
                mDo = true;
                mClip = null;
                return true;//重启录音时，返回true
            }
        }
        else
            return false;//不能录音时，返回false
    }

    void EndRecord(string tip = "")
    {
        Microphone.End(devices[0]);
        mTip = tip;
        mClipStr = AudioToStr(mClip);
        mStart = false;
        mDo = false;
    }

    //----------------------------------------------------

    public static string AudioToStr(AudioClip clip) //转换成字符串，利于存储
    {
        return ValueConvert.BytesToStr(AudioToBytes(clip));
    }

    public static AudioClip StrToAudio(string carrier,int freqCorr)
    {
        return BytesToAudio(ValueConvert.StrToBytes(carrier), freqCorr);
    }

    public static byte[] AudioToBytes(AudioClip clip)//转换成字节数组，利于传输
    {
        //AudioClip可以通过GetData和SetData获取和设置音频数据，但是数据是-1到1之间的float数组
        //因此byte[]在转到AudioClip时需要将数据缩放成-1到1之前的float
        //byte[]的两个字节对应Unity中一个float数据

        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        short[] intData = new short[samples.Length];//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]
        byte[] bytesData = new byte[samples.Length * 2];//bytesData array is twice the size of
                                                        //dataSource array because a float converted in Int16 is 2 bytes.
        int rescaleFactor = 32767;//to convert float to Int16
        //因为float数据在-1到1之前，我们需要把数据转换到有符号2个字节的范围 -32768~32767。因此这里乘以32767

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }

    public static AudioClip BytesToAudio(byte[] rawData,int freqCorr)
    {
        //注意，应该只支持利用audio clip本身所得数据而转换到的字节流，不支持音频文件字节流到clip的转换
        //不过clip本身给出的数据，就是纯音频，与pcm格式直接对应

        float[] samples = new float[rawData.Length / 2];
        float rescaleFactor = 32767;
        short st = 0;
        float ft = 0;

        for (int i = 0; i < rawData.Length; i += 2)
        {
            st = BitConverter.ToInt16(rawData, i);
            ft = st / rescaleFactor;
            samples[i / 2] = ft;
        }

        AudioClip audioClip = AudioClip.Create("mySound", samples.Length, 1, freqCorr, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }

    //------------------------------

    public struct Info
    {
        public int limit;//外界使用该组件的录音功能前，需要赋值
        public int frequency;//这个可能需要随情况调变
        public Action WhenFinish;//外界可用可不用

        public Info(int mFreq)
        {
            limit = 2;
            frequency = mFreq;
            WhenFinish = null;
        }
    }

}