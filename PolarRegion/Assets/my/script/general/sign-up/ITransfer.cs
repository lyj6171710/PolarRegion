using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//====================================

public interface Ifo { }//运行时，一次性给予对外的信息集

public interface Sto { }//需要存储数据的存储结构

public interface IForIfo<T> where T : Ifo //可以降低转换难度，只要一个类的每个成员都继承该接口，就可以分层转换，并且更易于检查
{
    T ToIfo();//从sto信息构成ifo信息，理应取得一个新对象
}

public interface ICanSto<T> where T : Sto
{
    T ToSto();//从ifo信息构成sto信息，理应取得一个新对象
}

//====================================

public interface IGetIfo<T, F>//被读取某方面的信息
{
    T GetIfo(F refer);
    //依据某种需要F
    //从ifo信息构成另一种ifo信息
}

public interface IGetIfo<T>
{
    T GetIfo();
}

//====================================