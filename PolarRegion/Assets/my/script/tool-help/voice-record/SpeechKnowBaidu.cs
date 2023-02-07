/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using LitJson;

public class SpeechKnowBaidu : MonoBehaviour
{//�������ȫ��ֻ����һ��

    [Header("����������������ƾ֤����������")]
    public string APP_ID = "25750403";
    public string API_KEY = "LneL4Kh6rGQnfZfGsp3N6Znj";
    public string SECRET_KEY = "ygpYce92PFuuhrcoWBd7h8HnPyGH25Zl";

    [Header("�����ϸ����������ĸ���")]
    public string UrlToGetToken = "https://aip.baidubce.com/oauth/2.0/token";//��ȡtocken�������ַ���᷵��json���ݣ�ÿ�λ�ȡtoken���ݶ���һ��
    public string UrlToSpeechRecognition = "https://vop.baidu.com/server_api";// ����ʶ���ַ

    //-----------------------------------------

    public static int meFreqAsk { get { return mRecordFreq; } }
    public int meLengthLimit { get { return mAudioTimeMax; } }

    //-----------------------------------------

    string mAccessToken = "";// ��ȡ����Token
    bool mIsTocken = false;

    int mAudioTimeMax = 60;//���ܳ����ٷ��ӿ����������Ҫ��
    const int mRecordFreq = 16000;//�����Ӳ��Ҫ�󣬸��ݹٷ��ӿڵ�����

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
            Debug.Log("����Ƶ�ʹ���");
            return;
        }
        else if (!mInIdentify && mIsTocken) 
        {
            mKind = kind;
            mWhenKnow = whenKnow;
            mInIdentify = true;//ֱ���˴����������ǰ���������ٴ����øù���
            StartCoroutine(Recognize(ClipToSpecPCM(need)));
        }
    }

    //-----------------------------------------------------------------------

    void GetToken(string url)//��ȡtoken
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "client_credentials");
        form.AddField("client_id", API_KEY);
        form.AddField("client_secret", SECRET_KEY);

        StartCoroutine(HttpPostRequest(url, form));
    }

    IEnumerator HttpPostRequest(string url, WWWForm form)//http����
    {//url�ǵ�ַ��form�ǲ���
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, form);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result==UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("�������:" + unityWebRequest.error);
        }
        else
        {
            if (unityWebRequest.responseCode == 200)
            {
                string result = unityWebRequest.downloadHandler.text;
                print("�ɹ���ȡ������:" + result);
                OnGetHttpResponseSuccess(result);
            }
            else
            {
                print("״̬�벻Ϊ200:" + unityWebRequest.responseCode);
            }
        }
    }

    void OnGetHttpResponseSuccess(string result)//���ɹ���ȡ�����������ص�json����ʱ,���н���
    {//resultҪ��json��ʽ������

        AccessToken inform = JsonMapper.ToObject<AccessToken>(result);
        if (inform == null)
            Debug.Log("��֤ʧ��");
        else
        {
            mAccessToken = inform.access_token;
            mIsTocken = true;
        }
    }

    //========================================================

    IEnumerator Recognize(byte[] audioData)//����ʶ��
    {
        WWWForm form = new WWWForm();
        //�������ݷ��� HTTP BODY��
        form.AddBinaryData("audio", audioData);
        int realizeFor = 0;
        switch (mKind)
        {
            case Kind.��ͨ��:realizeFor = 1537;break;//�����Ľӿ�˵���У�1537��ͨ����1737Ӣ�1837�Ĵ���
            case Kind.Ӣ��:realizeFor = 1737;break;
            case Kind.�Ĵ���:realizeFor = 1837;break;
        }
        //���Ʋ����Լ����ͳ����Ϣͨ�� header �� url ��Ĳ�������
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
                Debug.Log("��������������" + e);
                mWhenKnow("");
                break;
            };
            yield return wait;
        } while (!wait.isDone);

        if (request.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("�������:" + request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                try
                {
                    string result = request.downloadHandler.text;
                    print("�ɹ���ȡ������:" + result);

                    RecognizeResult resultContent = JsonMapper.ToObject<RecognizeResult>(result);
                    mWhenKnow(resultContent.result[0]);
                }
                catch (Exception e)
                {
                    Debug.Log("��������������" + e);
                    mWhenKnow("");
                }
            }
            else
            {
                print("״̬�벻Ϊ200:" + request.responseCode);
            }
        }
        mInIdentify = false;//����ʶ��
        mInterval = 0.2f;
    }

    static byte[] ClipToSpecPCM(AudioClip clip)
    {
        //��Unity��AudioClip����ת��ΪPCM��ʽ16000Ƶ�ʡ�16bit������������(�ٷ��ӿ���Ҫ�������ݣ�����audioclip����͵��Ѿ�������Щ��׼)

        float[] samples = new float[clip.samples * clip.channels];
        // = new float[16000 * clip.length * 1]
        //c#�У�һ��float��4���ֽڳ��������ȸ���
        clip.GetData(samples, 0);
        short[] samples_int16 = new short[samples.Length];
        //c#�У�һ��short��2���ֽڳ�(16λ)��������

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

    class AccessToken// AccessToken���л�json�Ķ���
    {
        public string access_token;

        public int expires_in;

        public string session_key;

        public string scope;

        public string refresh_token;

        public string session_secret;

        /*���հ���
        {
        "access_token": "24.b243f17d64fa69b413d827f6a0965846.2592000.1542375343.282335-14131279",
        "session_key": "9mzdWWhYL0oUaqTY7WohNY0Fhd8Wxm4M7t4bTtlaq9/fyw7RXgztqR8+tmnAFpgywswOL3CQsU/v6PZ3ijK91/RmmiLb9Q==",
        "scope": "audio_voice_assistant_get audio_tts_post public brain_all_scope wise_adapt lebo_resource_base lightservice_public hetu_basic lightcms_map_poi kaidian_kaidian ApsMisTest_TestȨ�� vis-classify_flower lpq_���� cop_helloScope ApsMis_fangdi_permission smartapp_snsapi_base iop_autocar oauth_tp_app smartapp_smart_game_openapi oauth_sessionkey",
        "refresh_token": "25.c2cf87484f244b6ef3d1d6a330727700.315360000.1855143343.282335-14131279",
        "session_secret": "7b9a68a03cbad17db3d13985dc7690d2",
        "expires_in": 2592000
        */
/*
    }

    class RecognizeResult// ����ʶ��ɹ���,���ص�json���ݸ�ʽ
    {
        public string corpus_no;

        public string err_msg;

        public int err_no;

        public List<string> result;

        public string sn;

        //���հ�����{"corpus_no":"6612962645817945596","err_msg":"success.","err_no":0,"result":["�������"],"sn":"845877030391539700349"}
    }

    public enum Kind { ��ͨ��, Ӣ�� ,�Ĵ��� }

    public static SpeechKnowBaidu It;
    void Awake()
    {
        It = this;
        mIsTocken = false;
    }
}
*/