using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataFileRefer : MonoBehaviour
{
//    //������=========================

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
//        //���л����̣���Save����ת��Ϊ�ֽ�����
//        //����Save���󲢱��浱ǰ��Ϸ״̬
//        Save save = CreateSaveGO();
//        //����һ�������Ƹ�ʽ������
//        BinaryFormatter bf = new BinaryFormatter();
//        //����һ���ļ���
//        FileStream fileStream = File.Create(Application.dataPath + "/StreamingFile" + "/byBin.txt");
//        //�ö����Ƹ�ʽ����������л����������л�Save����,�������������ļ�������Ҫ���л��Ķ���
//        bf.Serialize(fileStream, save);
//        //�ر���
//        fileStream.Close();

//        //����ļ����ڣ�����ʾ����ɹ�
//        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
//        {
//            UIManager._instance.ShowMessage("����ɹ�");
//        }
//    }

//    private void LoadByBin()
//    {
//        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
//        {
//            //�����л�����
//            //����һ�������Ƹ�ʽ������
//            BinaryFormatter bf = new BinaryFormatter();
//            //��һ���ļ���
//            FileStream fileStream = File.Open(Application.dataPath + "/StreamingFile" + "/byBin.txt", FileMode.Open);
//            //���ø�ʽ������ķ����л����������ļ���ת��Ϊһ��Save����
//            Save save = (Save)bf.Deserialize(fileStream);
//            //�ر��ļ���
//            fileStream.Close();

//            SetGame(save);
//            UIManager._instance.ShowMessage("");
//        }
//        else
//        {
//            UIManager._instance.ShowMessage("�浵�ļ�������");
//        }
//    }

//    //xml���============================

//    private void SaveByXml()
//    {
//        Save save = CreateSaveGO();
//        //����XML�ļ��Ĵ洢·��
//        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
//        //����XML�ĵ�
//        XmlDocument xmlDoc = new XmlDocument();
//        //�������ڵ㣬�����ϲ�ڵ�
//        XmlElement root = xmlDoc.CreateElement("save");
//        //���ø��ڵ��е�ֵ
//        root.SetAttribute("name", "saveFile1");

//        //����XmlElement
//        XmlElement target;
//        XmlElement targetPosition;
//        XmlElement monsterType;

//        //����save�д洢�����ݣ�������ת����XML��ʽ
//        for (int i = 0; i < save.livingTargetPositions.Count; i++)
//        {
//            target = xmlDoc.CreateElement("target");
//            targetPosition = xmlDoc.CreateElement("targetPosition");
//            //����InnerTextֵ
//            targetPosition.InnerText = save.livingTargetPositions[i].ToString();
//            monsterType = xmlDoc.CreateElement("monsterType");
//            monsterType.InnerText = save.livingMonsterTypes[i].ToString();

//            //���ýڵ��Ĳ㼶��ϵ root -- target -- (targetPosition, monsterType)
//            target.AppendChild(targetPosition);
//            target.AppendChild(monsterType);
//            root.AppendChild(target);
//        }

//        //����������ͷ����ڵ㲢���ò㼶��ϵ  xmlDoc -- root --(target-- (targetPosition, monsterType), shootNum, score)
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
//            UIManager._instance.ShowMessage("����ɹ�");
//        }
//    }

//    private void LoadByXml()
//    {
//        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
//        if (File.Exists(filePath))
//        {
//            Save save = new Save();
//            //����XML�ĵ�
//            XmlDocument xmlDoc = new XmlDocument();
//            xmlDoc.Load(filePath);

//            //ͨ���ڵ���������ȡԪ�أ����ΪXmlNodeList����
//            XmlNodeList targets = xmlDoc.GetElementsByTagName("target");
//            //�������е�target�ڵ㣬������ӽڵ���ӽڵ��InnerText
//            if (targets.Count != 0)
//            {
//                foreach (XmlNode target in targets)
//                {
//                    XmlNode targetPosition = target.ChildNodes[0];
//                    int targetPositionIndex = int.Parse(targetPosition.InnerText);
//                    //�ѵõ���ֵ�洢��save��
//                    save.livingTargetPositions.Add(targetPositionIndex);

//                    XmlNode monsterType = target.ChildNodes[1];
//                    int monsterTypeIndex = int.Parse(monsterType.InnerText);
//                    save.livingMonsterTypes.Add(monsterTypeIndex);
//                }
//            }

//            //�õ��洢��������ͷ���
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
//            UIManager._instance.ShowMessage("�浵�ļ�������");
//        }
//    }

//    //json���===========================

//    private void SaveByJson()
//    {
//        Save save = CreateSaveGO();
//        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
//        //����JsonMapper��save����ת��ΪJson��ʽ���ַ���
//        string saveJsonStr = JsonMapper.ToJson(save);
//        //������ַ���д�뵽�ļ���
//        //����һ��StreamWriter�������ַ���д���ļ���
//        StreamWriter sw = new StreamWriter(filePath);
//        sw.Write(saveJsonStr);
//        //�ر�StreamWriter
//        sw.Close();

//        UIManager._instance.ShowMessage("����ɹ�");
//    }

//    private void LoadByJson()
//    {
//        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
//        if (File.Exists(filePath))
//        {
//            //����һ��StreamReader��������ȡ��
//            StreamReader sr = new StreamReader(filePath);
//            //����ȡ��������ֵ��jsonStr
//            string jsonStr = sr.ReadToEnd();
//            //�ر�
//            sr.Close();

//            //���ַ���jsonStrת��ΪSave����
//            Save save = JsonMapper.ToObject<Save>(jsonStr);
//            SetGame(save);
//            UIManager._instance.ShowMessage("");
//        }
//        else
//        {
//            UIManager._instance.ShowMessage("�浵�ļ�������");
//        }
//    }
}
