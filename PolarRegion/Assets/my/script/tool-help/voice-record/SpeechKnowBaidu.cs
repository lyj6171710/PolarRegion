/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using LitJson;

public class SpeechKnowBaidu : MonoBehaviour
{//该组件，全局只存在一个

    [Header("官网上申请下来的凭证，不能随便给")]
    public string APP_ID = "25750403";
    public string API_KEY = "LneL4Kh6rGQnfZfGsp3N6Znj";
    public string SECRET_KEY = "ygpYce92PFuuhrcoWBd7h8HnPyGH25Zl";

    [Header("官网上给出的渠道的复制")]
    public string UrlToGetToken = "https://aip.baidubce.com/oauth/2.0/token";//获取tocken的请求地址，会返回json数据，每次获取token数据都不一样
    public string UrlToSpeechRecognition = "https://vop.baidu.com/server_api";// 语音识别地址

    //-----------------------------------------

    public static int meFreqAsk { get { return mRecordFreq; } }
    public int meLengthLimit { get { return mAudioTimeMax; } }

    //-----------------------------------------

    string mAccessToken = "";// 获取到的Token
    bool mIsTocken = false;

    int mAudioTimeMax = 60;//不能超过官方接口需求的上限要求
    const int mRecordFreq = 16000;//这个是硬性要求，根据官方接口的需求

    Kind mKind;
    Action<string> mWhenKnow;
    bool mInIdentify = false;
    float mInterval;

    void Start()
    {
        GetToken(UrlToGetToken);
    }

    void Update()
    {
        if (mInterval > 0)
            mInterval -= Time.deltaTime;
    }

    public void MakeRealize(AudioClip need, Kind kind, Action<string> whenKnow)
    {
        if (mInterval > 0)
        {
            Debug.Log("请求频率过大");
            return;
        }
        else if (!mInIdentify && mIsTocken) 
        {
            mKind = kind;
            mWhenKnow = whenKnow;
            mInIdentify = true;//直到此次请求处理完毕前，都不能再次启用该功能
            StartCoroutine(Recognize(ClipToSpecPCM(need)));
        }
    }

    //-----------------------------------------------------------------------

    void GetToken(string url)//获取token
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "client_credentials");
        form.AddField("client_id", API_KEY);
        form.AddField("client_secret", SECRET_KEY);

        StartCoroutine(HttpPostRequest(url, form));
    }

    IEnumerator HttpPostRequest(string url, WWWForm form)//http请求
    {//url是地址，form是参数
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, form);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result==UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("网络错误:" + unityWebRequest.error);
        }
        else
        {
            if (unityWebRequest.responseCode == 200)
            {
                string result = unityWebRequest.downloadHandler.text;
                print("成功获取到数据:" + result);
                OnGetHttpResponseSuccess(result);
            }
            else
            {
                print("状态码不为200:" + unityWebRequest.responseCode);
            }
        }
    }

    void OnGetHttpResponseSuccess(string result)//当成功获取到服务器返回的json数据时,进行解析
    {//result要是json格式的内容

        AccessToken inform = JsonMapper.ToObject<AccessToken>(result);
        if (inform == null)
            Debug.Log("认证失败");
        else
        {
            mAccessToken = inform.access_token;
            mIsTocken = true;
        }
    }

    //========================================================

    IEnumerator Recognize(byte[] audioData)//语音识别
    {
        WWWForm form = new WWWForm();
        //语音数据放在 HTTP BODY中
        form.AddBinaryData("audio", audioData);
        int realizeFor = 0;
        switch (mKind)
        {
            case Kind.普通话:realizeFor = 1537;break;//官网的接口说明中，1537普通话、1737英语、1837四川话
            case Kind.英语:realizeFor = 1737;break;
            case Kind.四川话:realizeFor = 1837;break;
        }
        //控制参数以及相关统计信息通过 header 和 url 里的参数传递
        string url = string.Format("{0}?dev_pid={1}&cuid={2}&token={3}", UrlToSpeechRecognition, realizeFor, SystemInfo.deviceUniqueIdentifier, mAccessToken);
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("Content-Type", "audio/pcm;rate=16000");

        UnityWebRequestAsyncOperation wait;
        do
        {
            try { 
                wait = request.SendWebRequest();
            }
            catch (Exception e) {
                Debug.Log("第三方出问题了" + e);
                mWhenKnow("");
                break;
            };
            yield return wait;
        } while (!wait.isDone);

        if (request.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("网络错误:" + request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                try
                {
                    string result = request.downloadHandler.text;
                    print("成功获取到数据:" + result);

                    RecognizeResult resultContent = JsonMapper.ToObject<RecognizeResult>(result);
                    mWhenKnow(resultContent.result[0]);
                }
                catch (Exception e)
                {
                    Debug.Log("第三方出问题了" + e);
                    mWhenKnow("");
                }
            }
            else
            {
                print("状态码不为200:" + request.responseCode);
            }
        }
        mInIdentify = false;//结束识别
        mInterval = 0.2f;
    }

    static byte[] ClipToSpecPCM(AudioClip clip)
    {
        //将Unity的AudioClip数据转化为PCM格式16000频率、16bit、单声道数据(官方接口需要这种数据，不过audioclip本身就得已经符合这些标准)

        float[] samples = new float[clip.samples * clip.channels];
        // = new float[16000 * clip.length * 1]
        //c#中，一个float，4个字节长，单精度浮点
        clip.GetData(samples, 0);
        short[] samples_int16 = new short[samples.Length];
        //c#中，一个short，2个字节长(16位)，短整型

        for (var index = 0; index < samples.Length; index++)
        {
            float f = samples[index];
            samples_int16[index] = (short)(f * short.MaxValue);
        }

        byte[] byteArray = new byte[samples_int16.Length * 2];
        Buffer.BlockCopy(samples_int16, 0, byteArray, 0, byteArray.Length);

        return byteArray;
    }

    //=======================================================

    class AccessToken// AccessToken序列化json的对象
    {
        public string access_token;

        public int expires_in;

        public string session_key;

        public string scope;

        public string refresh_token;

        public string session_secret;

        /*接收案例
        {
        "access_token": "24.b243f17d64fa69b413d827f6a0965846.2592000.1542375343.282335-14131279",
        "session_key": "9mzdWWhYL0oUaqTY7WohNY0Fhd8Wxm4M7t4bTtlaq9/fyw7RXgztqR8+tmnAFpgywswOL3CQsU/v6PZ3ijK91/RmmiLb9Q==",
        "scope": "audio_voice_assistant_get audio_tts_post public brain_all_scope wise_adapt lebo_resource_base lightservice_public hetu_basic lightcms_map_poi kaidian_kaidian ApsMisTest_Test权限 vis-classify_flower lpq_开放 cop_helloScope ApsMis_fangdi_permission smartapp_snsapi_base iop_autocar oauth_tp_app smartapp_smart_game_openapi oauth_sessionkey",
        "refresh_token": "25.c2cf87484f244b6ef3d1d6a330727700.315360000.1855143343.282335-14131279",
        "session_secret": "7b9a68a03cbad17db3d13985dc7690d2",
        "expires_in": 2592000
        */
/*
    }

    class RecognizeResult// 语音识别成功后,返回的json数据格式
    {
        public string corpus_no;

        public string err_msg;

        public int err_no;

        public List<string> result;

        public string sn;

        //接收案例：{"corpus_no":"6612962645817945596","err_msg":"success.","err_no":0,"result":["你今年多大，"],"sn":"845877030391539700349"}
    }

    public enum Kind { 普通话, 英语 ,四川话 }

    public static SpeechKnowBaidu It;
    void Awake()
    {
        It = this;
        mIsTocken = false;
    }
}
*/