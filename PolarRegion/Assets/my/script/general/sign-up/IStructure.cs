using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBase { }

public interface ISignUpRoomRoot //��״�ݹ�
{
    int SignRoomRoot { get; }
}

interface ISURoomTree//����������Ҷ��������չ����ƣ����ں�����Ҷ�������ϵĲ�ͬ
                     //����֦�ɲ�ͬ�������ʸ���֦�ɺͷ���֦�ɵ�Ҷ�ӣ���Ϊ������һ����
{
    Transform iTrunk { get; }//�Լ�������

    Dictionary<Vector2Int, RoomBase> iRooms { get; set; }//�Լ���Ҷ��

    RoomBase iRoot { get; set; }//�Լ����ڵĸ�

    List<Vector2Int> iRoute { get; set; }//�Ӹ����Լ���·�ߣ�������û������

    RoomBase IAccessRoom(List<Vector2Int> route);
    //����ĳ���ڵ�ķ���
    //������Ӧ���ı�
    //���ʷ��䣬û�еĻ����ȴ���������Ȼ�󷵻�
    //�����ʹ�ã���Ϊ��粻��ȷ����Ӧ�������

    void IBackFromRoom(Vector2Int at, Enum msg);
}