using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISignUpClearSelf
{
}

interface ISUClearSelf
{
    void DelSelfFile();//删除存储自己信息的硬盘文件

    void DelRelaFile();//删除包含对自己有引用的硬盘文件中的那部分信息

    void DelOtherCite();//删除内存中，引用有自己的对象中的那部分信息

    void DelOtherUse();//关闭运行过程中，因为自己而引发或产生的效果
}
