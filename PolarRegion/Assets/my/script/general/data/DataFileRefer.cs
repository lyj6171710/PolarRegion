using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataFileRefer : MonoBehaviour
{
//    //二进制=========================

//    [System.Serializable]
//    public class Save
//    {

//        public List<int> livingTargetPositions = new List<int>();
//        public List<int> livingMonsterTypes = new List<int>();

//        public int shootNum = 0;
//        public int score = 0;
//    }

//    private void SaveByBin()
//    {
//        //序列化过程（将Save对象转换为字节流）
//        //创建Save对象并保存当前游戏状态
//        Save save = CreateSaveGO();
//        //创建一个二进制格式化程序
//        BinaryFormatter bf = new BinaryFormatter();
//        //创建一个文件流
//        FileStream fileStream = File.Create(Application.dataPath + "/StreamingFile" + "/byBin.txt");
//        //用二进制格式化程序的序列化方法来序列化Save对象,参数：创建的文件流和需要序列化的对象
//        bf.Serialize(fileStream, save);
//        //关闭流
//        fileStream.Close();

//        //如果文件存在，则显示保存成功
//        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
//        {
//            UIManager._instance.ShowMessage("保存成功");
//        }
//    }

//    private void LoadByBin()
//    {
//        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
//        {
//            //反序列化过程
//            //创建一个二进制格式化程序
//            BinaryFormatter bf = new BinaryFormatter();
//            //打开一个文件流
//            FileStream fileStream = File.Open(Application.dataPath + "/StreamingFile" + "/byBin.txt", FileMode.Open);
//            //调用格式化程序的反序列化方法，将文件流转换为一个Save对象
//            Save save = (Save)bf.Deserialize(fileStream);
//            //关闭文件流
//            fileStream.Close();

//            SetGame(save);
//            UIManager._instance.ShowMessage("");
//        }
//        else
//        {
//            UIManager._instance.ShowMessage("存档文件不存在");
//        }
//    }

//    //xml存读============================

//    private void SaveByXml()
//    {
//        Save save = CreateSaveGO();
//        //创建XML文件的存储路径
//        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
//        //创建XML文档
//        XmlDocument xmlDoc = new XmlDocument();
//        //创建根节点，即最上层节点
//        XmlElement root = xmlDoc.CreateElement("save");
//        //设置根节点中的值
//        root.SetAttribute("name", "saveFile1");

//        //创建XmlElement
//        XmlElement target;
//        XmlElement targetPosition;
//        XmlElement monsterType;

//        //遍历save中存储的数据，将数据转换成XML格式
//        for (int i = 0; i < save.livingTargetPositions.Count; i++)
//        {
//            target = xmlDoc.CreateElement("target");
//            targetPosition = xmlDoc.CreateElement("targetPosition");
//            //设置InnerText值
//            targetPosition.InnerText = save.livingTargetPositions[i].ToString();
//            monsterType = xmlDoc.CreateElement("monsterType");
//            monsterType.InnerText = save.livingMonsterTypes[i].ToString();

//            //设置节点间的层级关系 root -- target -- (targetPosition, monsterType)
//            target.AppendChild(targetPosition);
//            target.AppendChild(monsterType);
//            root.AppendChild(target);
//        }

//        //设置射击数和分数节点并设置层级关系  xmlDoc -- root --(target-- (targetPosition, monsterType), shootNum, score)
//        XmlElement shootNum = xmlDoc.CreateElement("shootNum");
//        shootNum.InnerText = save.shootNum.ToString();
//        root.AppendChild(shootNum);

//        XmlElement score = xmlDoc.CreateElement("score");
//        score.InnerText = save.score.ToString();
//        root.AppendChild(score);

//        xmlDoc.AppendChild(root);
//        xmlDoc.Save(filePath);

//        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byXML.txt"))
//        {
//            UIManager._instance.ShowMessage("保存成功");
//        }
//    }

//    private void LoadByXml()
//    {
//        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
//        if (File.Exists(filePath))
//        {
//            Save save = new Save();
//            //加载XML文档
//            XmlDocument xmlDoc = new XmlDocument();
//            xmlDoc.Load(filePath);

//            //通过节点名称来获取元素，结果为XmlNodeList类型
//            XmlNodeList targets = xmlDoc.GetElementsByTagName("target");
//            //遍历所有的target节点，并获得子节点和子节点的InnerText
//            if (targets.Count != 0)
//            {
//                foreach (XmlNode target in targets)
//                {
//                    XmlNode targetPosition = target.ChildNodes[0];
//                    int targetPositionIndex = int.Parse(targetPosition.InnerText);
//                    //把得到的值存储到save中
//                    save.livingTargetPositions.Add(targetPositionIndex);

//                    XmlNode monsterType = target.ChildNodes[1];
//                    int monsterTypeIndex = int.Parse(monsterType.InnerText);
//                    save.livingMonsterTypes.Add(monsterTypeIndex);
//                }
//            }

//            //得到存储的射击数和分数
//            XmlNodeList shootNum = xmlDoc.GetElementsByTagName("shootNum");
//            int shootNumCount = int.Parse(shootNum[0].InnerText);
//            save.shootNum = shootNumCount;

//            XmlNodeList score = xmlDoc.GetElementsByTagName("score");
//            int scoreCount = int.Parse(score[0].InnerText);
//            save.score = scoreCount;

//            SetGame(save);
//            UIManager._instance.ShowMessage("");

//        }
//        else
//        {
//            UIManager._instance.ShowMessage("存档文件不存在");
//        }
//    }

//    //json存读===========================

//    private void SaveByJson()
//    {
//        Save save = CreateSaveGO();
//        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
//        //利用JsonMapper将save对象转换为Json格式的字符串
//        string saveJsonStr = JsonMapper.ToJson(save);
//        //将这个字符串写入到文件中
//        //创建一个StreamWriter，并将字符串写入文件中
//        StreamWriter sw = new StreamWriter(filePath);
//        sw.Write(saveJsonStr);
//        //关闭StreamWriter
//        sw.Close();

//        UIManager._instance.ShowMessage("保存成功");
//    }

//    private void LoadByJson()
//    {
//        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
//        if (File.Exists(filePath))
//        {
//            //创建一个StreamReader，用来读取流
//            StreamReader sr = new StreamReader(filePath);
//            //将读取到的流赋值给jsonStr
//            string jsonStr = sr.ReadToEnd();
//            //关闭
//            sr.Close();

//            //将字符串jsonStr转换为Save对象
//            Save save = JsonMapper.ToObject<Save>(jsonStr);
//            SetGame(save);
//            UIManager._instance.ShowMessage("");
//        }
//        else
//        {
//            UIManager._instance.ShowMessage("存档文件不存在");
//        }
//    }
}
