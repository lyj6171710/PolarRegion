using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotAssist:MonoBehaviour
{
    //��tranform��eulerAngles��ֵ������-180��180��Χ��ʱ������ᱻ�Ȼ��������Χ�ڣ�������ȷ���ǣ�������-180ʱ������

    public void Sample()
    {
        //1.��Ԫ��ת����ŷ����

        Vector3 v3 = transform.rotation.eulerAngles;
        
        //2.��Ԫ��ת���ɷ�������

        Vector3 vector3 = (transform.rotation * Vector3.forward).normalized;
        
        //3.ŷ����ת������Ԫ��

        Quaternion rotation = Quaternion.Euler(vector3);
        
        //4.ŷ����ת���ɷ�������

        Vector3 v4 = (Quaternion.Euler(vector3) * Vector3.forward).normalized;
        
        //5.����������ת��Ϊ��Ԫ��

        Quaternion rotation2 = Quaternion.LookRotation(vector3);
        
        //6.����������ת��Ϊŷ����

        Vector3 v5 = Quaternion.LookRotation(vector3).eulerAngles;
    }

}
