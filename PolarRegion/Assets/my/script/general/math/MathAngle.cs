using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathAngle
{
    //�㷨˼·�ǣ�
    //Ϊ������˼����ֱ棬���ⷴ����ֵ��һ����������360֮��
    //��������Ϊ��ȣ���ʱ��Ϊ����˳ʱ��Ϊ���ı���

    //�Ƕ����===================================================================
    public static float AngleNip(Vector2 high, Vector2 low)
    {//�����������ļн�,ȡ���߶�Ӧ������ߵļн�Ϊ0����ʱ��Ϊ����,����Ϊ180��,����-180��
        float radian_high = GetRadian(high);
        float radian_low = GetRadian(low);
        float radian_nip;
        //�ֱ������Ӧ���Ⱥ�����û��Ȳ�
        //ע������low��ʼ����нǣ���Ҫ��������

        float temp_nip = radian_high - radian_low;
        //���Ȳ������޷ֱ�Ϊ2*Mathf.PI��-2*Mathf.PI

        if (temp_nip > Mathf.PI && radian_low < radian_high)
            radian_nip = temp_nip - 2 * Mathf.PI;
        //��ʱhigh���·���low��high�����Ϸ�

        else if (temp_nip < -Mathf.PI && radian_low > radian_high)
            radian_nip = 2 * Mathf.PI + temp_nip;
        //��ʱhigh���Ϸ���low��high�����·�

        else//-Mathf.PI <= temp_nip <= Mathf.PI
            radian_nip = temp_nip;
        //��ʱhigh���·���low��high�����Ϸ������·������·�
        //��ʱhigh���Ϸ���low��high�����·������Ϸ������Ϸ�

        return GetAngle(radian_nip);
        //���Ȳ��Ӧֵ����������нǶ�Ӧ�Ļ���ֵ,�ɴ˿���ó��Ƕ���
    }
    public static float GetAngle(float radian)
    {//���������ȣ���������Ӧ����
        return GetSameAngle360(radian * 180 / Mathf.PI);
    }
    public static float GetAngle(Vector2 ask)
    {//Ĭ�Ϸ��ؿɶ�Ӧ�������Ƕȣ�û��������׼ʱ��ͳһΪ0��360��
        float angle = Vector2.Angle(ask, Vector2.right);
        Vector3 side = Vector3.Cross(ask, Vector3.right);
        if (side.z > 0) angle = 360 - angle;
        return angle;
    }
    public static float GetSameAngle360(float angle)
    {//���ر����ϣ�ָ���Ƕ�����Ч�ģ�����ֵ�����360�ȵĶ���
        return MathNum.CountRemainder(angle, 360);//ȥ��������
    }

    public static float GetSameAngle180(float angle)
    {//���Ƕȱ�����-179��180��֮��
        float angle360 = Mathf.Abs(GetSameAngle360(angle));
        if (angle360 > 180) return angle360 - 360;
        else if (angle360 <= -180) return angle360 + 360;
        else return angle360;

    }

    //�������===================================================================
    //����get_radian����ֻ�ᷴ�����������ܵ�Ч��
    //����360�ȵ�����������Ӧ�Ļ��ȣ�Ҳ�㹻�������з���
    public static float GetRadian(Vector2 ask)//��ԭ����ָ����������ߣ���x�������ߵļнǻ���
    {//Mathf��arctan������Ѷ�Ӧ���꣬���ĶԳƵ�cos��������������(��ʵ����Ϊ���ĶԳƵ���������Ӧtanֵ��ͬ����)
     //arctan������ֵ����б��������ҷ��Ļ��ȣ��ҷ�ΧΪ-Mathf.PI/2��Mathf.PI/2
        float radian = Mathf.Atan(ask.y / ask.x);
        //tanֵ���������Ƿ��ǵ�λ����Ӱ�죬����������Ӱ��
        //����ע���ȷŵڶ�����ʽ����y���ٽ���д��һ������x
        if (ask.x >= 0 && ask.y >= 0)//��һ����
            return radian;
        else if (ask.x < 0 && ask.y < 0)//��������
            return radian + Mathf.PI;
        else if (ask.x >= 0 && ask.y < 0)//��������
            return 2 * Mathf.PI + radian;
        else//�ڶ�����
            return Mathf.PI + radian;
        //�˺��������нǶȿ����Ǵ����ҷ���ʱ����ת���ã��������ֲ�ͬ�Ƕ�
        //Ҳ��˷��صĻ��ȣ�����Ϊ2*Mathf.PI,����Ϊ0
    }
    public static float GetRadian(float angle)
    {//ÿһ�����ȶ�Ӧ�ض��Ƕȣ��ض��Ƕ��ֶ�Ӧ�ض�sin��cos��tanֵ
        angle = GetSameAngle360(angle);//��Ч��һ�������ڵĶ���
        return Mathf.PI * angle / 180;
    }

    public static float GetSameRadian(float radian)
    {
        return MathNum.CountRemainder(radian, 2 * Mathf.PI);
    }

    //�������===================================================================

    public static Vector2 AngleToVector(float angle)
    {
        float radian = GetRadian(angle);//��ת����
        return RadianToVector(radian);
    }

    public static Vector2 RadianToVector(float radian)
    {
        radian = GetSameRadian(radian);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 CoordTransfer(Vector2 start, Vector2 toward, float distance)
    {//���ش�ָ�����갴ָ�������н�ָ�����������������
        float radian = GetRadian(toward);
        float cos_radian = Mathf.Cos(radian);
        float sin_radian = Mathf.Sin(radian);
        Vector2 end;
        float end_x = start.x + distance * cos_radian;
        float end_y = start.y + distance * sin_radian;
        end = new Vector2(end_x, end_y);
        return end;
    }
    public static Vector2 CoordTransfer(Vector2 start, Vector2 offset)
    {
        Vector2 end;
        float end_x = start.x + offset.x;
        float end_y = start.y + offset.y;
        end = new Vector2(end_x, end_y);
        return end;
    }
    public static Vector2 CoordAgainst(Vector2 pos)
    {
        return new Vector2(-pos.x, -pos.y);
    }
    public static Vector2 CoordXAgainst(Vector2 pos)
    {
        return new Vector2(-pos.x, pos.y);
    }
    public static Vector2 CoordYAgainst(Vector2 pos)
    {
        return new Vector2(pos.x, -pos.y);
    }
    public static Vector2 OffsetByDir(float angle, float distance)
    {//��ָ���Ƕ���(�Ƕ��Զ�������)��ԭ��ǰ��ָ�������������λ�õ������(����ֵ��Գ�������ϵ����)
     //��·��Ϊ1ʱ��������������Ϊ�ýǶȶ�Ӧ��ֱ�Ǳ߹�ϵ����Ϊ��ʱб��Ϊ1��ͶӰ��x��y��ֵ������Ӧ����б�ߵı�ֵ,Ҳ������Ӧ�Ƕȵĵ�λ����
        float radian = GetRadian(angle);
        Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        return new Vector2(distance * direction.x, distance * direction.y);
    }
    public static Vector2 OffsetByDir(Vector2 toward, float distance)
    {//��ָ���Ƕ���(�Ƕ����������ֵ����)��ԭ��ǰ��ָ�������������λ�õ������
        float radian = GetRadian(toward);
        Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        return new Vector2(distance * direction.x, distance * direction.y);
    }
    public static Vector2 CoordRotate(Vector2 target, float angle)
    {//��ָ����������ԭ�������ת��������ԭ�㱣�־�������תָ���ǶȺ����ɵ������
        float tg_length = target.magnitude;//�󳤶�
        float tg_angle = GetAngle(target);//��Ƕ�
        float last_angle = tg_angle + angle;//����ת��Ƕ�
        return OffsetByDir(last_angle, tg_length);
    }

    //��ͨ�����������������Ĺ�ͨ����===============================

    public static int IsAcute(Vector2 one, Vector2 other)
    {//��������������������������н����
        float dot=Vector2.Dot(one, other);
        if (dot > 0) return 1;
        else if (MathNum.Approximately(dot, 0)) return 0;
        else return -1;
    }

    public static bool WhetherInAngle(Vector2 other, Vector2 self, Vector2 selfFaceTo, float angleLimit)
    {//���˺��Ƿ���ĳ���н���
        Vector2 vectorToOther = other - self;//�жϹ����н�
        float diverge = MathAngle.AngleNip(vectorToOther, selfFaceTo);
        if (diverge > angleLimit * 0.5f)
            return false;
        else
            return true;
    }

    public static float LerpAngle(float a, float b, float t)
    {//Same as Lerp but makes sure the values interpolate correctly when they wrap around
     //     360 degrees.
        float num = MathNum.Repeat(b - a, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return a + num * MathNum.Clamp01(t);
    }


    public static float DeltaAngle(float current, float target)
    {//Calculates the shortest difference between two given angles given in degrees.
        float num = MathNum.Repeat(target - current, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        return num;
    }

    public static float MoveTowardsAngle(float current, float target, float maxDelta)
    {//Same as MoveTowards but makes sure the values interpolate correctly when they
     //     wrap around 360 degrees.
        float num = DeltaAngle(current, target);
        if (0f - maxDelta < num && num < maxDelta)
        {
            return target;
        }

        target = current + num;
        return MathNum.MoveTowards(current, target, maxDelta);
    }

}
