using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotAssist:MonoBehaviour
{
    //对tranform的eulerAngles赋值，不在-180到180范围内时，好像会被等换到这个范围内，至少明确的是，当低于-180时会这样

    public void Sample()
    {
        //1.四元数转化成欧拉角

        Vector3 v3 = transform.rotation.eulerAngles;
        
        //2.四元数转化成方向向量

        Vector3 vector3 = (transform.rotation * Vector3.forward).normalized;
        
        //3.欧拉角转换成四元数

        Quaternion rotation = Quaternion.Euler(vector3);
        
        //4.欧拉角转换成方向向量

        Vector3 v4 = (Quaternion.Euler(vector3) * Vector3.forward).normalized;
        
        //5.将方向向量转换为四元数

        Quaternion rotation2 = Quaternion.LookRotation(vector3);
        
        //6.将方向向量转换为欧拉角

        Vector3 v5 = Quaternion.LookRotation(vector3).eulerAngles;
    }

}
