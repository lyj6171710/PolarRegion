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

public class PresetTrack : MonoBehaviour//路径信息收集与转换
{

    public List<IfoTransform> meTrackPoints;//轨迹数据为当前父物体意义是朝右情况下的轨迹

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
        for (int i = 0; i < trackRefer.Count; i++)//转换到信息集
        {
            IfoTransform ifo = new IfoTransform();

            ifo.pos = GbjAssist.GetLocalPosInScene(trackRefer[i]);
            //轨迹位置会随父物体位置变化，所以只需记录本地情况
            //这里获取到的相对距离，是相对场景的，不受父物体缩放影响
            ifo.rot = trackRefer[i].localEulerAngles;
            ifo.scale = trackRefer[i].localScale;

            meTrackPoints.Add(ifo);
            Destroy(trackRefer[i].gameObject);
        }
        trackRefer.Clear();

    }
}
