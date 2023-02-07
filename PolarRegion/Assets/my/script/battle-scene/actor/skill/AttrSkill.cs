using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttrSkill : MonoBehaviour
{
    public Ifo mMechanismDp;
    
    [System.Serializable]
    public class Ifo//��Ӱ�켼��Ч������Щ����
    {
        public float hpCost;//���Ķ���Ѫ��
        public float tpCost;//���Ķ�������
        public float cooling;//��ȴ
        public float range;//ʩ����Χ
        public bool moveLocal;//�����ɫ�ƶ���������Խ�ɫ�ƶ�
        public EPR_SkillTrack track;
        [MultEnum] public EPR_SkillEffectNature nature;
        public ECamp campTo;

        //================================
        
    }

    [System.Serializable]
    public class IfoAttach//�߼�����(��ʱ������)
    {
        public float preTime;//�������������Ч����ʱ��ǰҡ
        public float postTime;//�������Ч������ɫ�ص�������״̬��ʱ����ҡ
        //ǰҡ����ҡ�ڼ䣬һ������£����޷����������ж�
        public bool canBreak;//�����ܷ񱻴�ϣ����������ǰҡ�ڼ䱻�쳣�����ˣ�Ҳ����������������Ч��������ͻᱻ��ϣ�û�м���Ч��

    }
}

