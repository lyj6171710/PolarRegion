using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultEnum : PropertyAttribute {//ö�ٱ������ɶ�ѡ��������

    public bool IfPick(Enum enumAsk, Enum enumState)
    {// �ж��Ƿ�ѡ�и�ö��ֵ
        int index = 1 << enumAsk.GetHashCode();//��1����
        int result = enumState.GetHashCode();//intֻ����洢�ռ������
        if ((result & index) == index)
        {
            return true;
        }
        return false;
    }

    int GetEnumInt(Enum type)//��ȡö��ֵ��Ӧ����ֵ
    {//�ú���ֻ����εõ���Ӧ��ֵ��ʾ����������ֱ������(���ת�����̼�ʱ)
        return type.GetHashCode();
    }
}

public enum EVoid { o }//��������ʾ��ֵ��ǿ������Ҫֵ

public enum ENum { one, two, three }

public enum EToward4 { up = 0, down = 1, left = 2, right = 3, middle = 4 }//����Ҫmiddle����Ϊ�����ĸ����򶼲��ǣ��͵���mid������
public enum EToward8 { up = 0, down = 1, left = 2, right = 3, middle = 4, upLeft = 5, upRight = 6, downLeft = 7, downRight = 8 }
public enum ETowardX4 { upLeft = 5, upRight = 6, downLeft = 7, downRight = 8 , middle = 9 }
public enum EFormOffset { percent, fix, to }//�ԣ��ٷֱ�/������������Ҫ�ı仯

public enum EKindAct { constant, accelerate, lerp }//��ô�仯��

public enum EDrift { none, horizontal, vertical, oblique }//���š����š�б�ţ���Ҫ����Ӧ����ֵʱ���������

public enum EMsgs //������Щ�����໥�������Ϣ�����ʺ�Ƶ�����ձ飬�ض���Ҫ����Щ��Ϣ
{ whether_alive_ = 0, whether_trigger_ = 1, audio_effect = 2, footfall_effect = 3, whether_ready_ = 4, tell_damage_ = 5 }

public enum EVarKind { boolean = 0, integer = 1, fraction = 2 , char_queue=3 }

public enum ECamp { amity, enemy, neutral }

public enum EMotion { constant, accelerate, decelerate, up_peak, down_peak }//һ���ƶ�ģʽ

public enum EAngle { degree, radian, vector }