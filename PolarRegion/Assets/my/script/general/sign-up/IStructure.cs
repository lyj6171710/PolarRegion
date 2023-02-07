using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBase { }

public interface ISignUpRoomRoot //树状递归
{
    int SignRoomRoot { get; }
}

interface ISURoomTree//从树根到树叶，单向延展与控制，不在乎根和叶子类型上的不同
                     //根和枝干不同，但访问根的枝干和访问枝干的叶子，行为可以是一样的
{
    Transform iTrunk { get; }//自己的载体

    Dictionary<Vector2Int, RoomBase> iRooms { get; set; }//自己的叶子

    RoomBase iRoot { get; set; }//自己所在的根

    List<Vector2Int> iRoute { get; set; }//从根到自己的路线，根本身没有坐标

    RoomBase IAccessRoom(List<Vector2Int> route);
    //外界对某个节点的访问
    //参数不应被改变
    //访问房间，没有的话会先创建出房间然后返回
    //供外界使用，因为外界不便确保相应房间存在

    void IBackFromRoom(Vector2Int at, Enum msg);
}