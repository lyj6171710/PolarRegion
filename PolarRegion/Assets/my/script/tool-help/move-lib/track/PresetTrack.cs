using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfoTransform
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
    public Vector3Int startFace;
}

public class PresetTrack : MonoBehaviour//·����Ϣ�ռ���ת��
{

    public List<IfoTransform> meTrackPoints;//�켣����Ϊ��ǰ�����������ǳ�������µĹ켣

    bool mHaveReady;

    public void MakeReady()
    {
        if (mHaveReady) return;

        List<Transform> trackRefer = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            trackRefer.Add(transform.GetChild(i));
        }
        Convert(trackRefer);

        mHaveReady = true;
    }

    void Convert(List<Transform> trackRefer)
    {
        meTrackPoints = new List<IfoTransform>();
        for (int i = 0; i < trackRefer.Count; i++)//ת������Ϣ��
        {
            IfoTransform ifo = new IfoTransform();

            ifo.pos = GbjAssist.GetLocalPosInScene(trackRefer[i]);
            //�켣λ�û��游����λ�ñ仯������ֻ���¼�������
            //�����ȡ������Ծ��룬����Գ����ģ����ܸ���������Ӱ��
            ifo.rot = trackRefer[i].localEulerAngles;
            ifo.scale = trackRefer[i].localScale;

            meTrackPoints.Add(ifo);
            Destroy(trackRefer[i].gameObject);
        }
        trackRefer.Clear();

    }
}
