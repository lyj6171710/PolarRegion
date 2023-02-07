using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//前说：A星算法是一种寻路算法，但也属于一种高层思想，只是说有机会得到最佳路径，
//它还需要一系列子算法，子算法最优时，A星算法才会最优，不过可能性能压力增大

/* A*算法的搜索原理，
 *（1）是广度优先搜索算法（BFS）的优化。从起点开始，首先遍历起点周围邻近的点，
 * 然后再遍历已经遍历过的点邻近的点，逐步的向外扩散，直到找到终点。
 *（2）采用贪心的策略，即“若A到C的最短路径经过B，则A到B的那一段必须取最短”，
 * 既然需要从B到C，那么A到B越短越好。由此找出起点到每个可能到达的点的最短路径并记录，直到覆盖到目标点。
 *（3）是一个“启发式”算法，它已经有了一些我们告诉它的先验知识，
 * 如“朝着终点的方向走更可能走到”。它不仅关注已走过的路径，还会对未走过的点或状态进行预测。*/



class MatrixMapPath
{
    //矩阵地图
    //A星算法应用情境广泛，这里演示在矩阵地图中的表现

    class PosInner
    {//内部使用的数据结构

        public Vector2Int coord;//这个很常用，还是得记录下来，而不是每次算一遍
        public int moveDfct;//这个位置可以走吗，移动难度，0时不能移动，
        //0以上可以移动，但难度随数值递增,但目前只考虑了可走与不可走

        public PosInner pre;//被走过时，记录行人上一刻所在的位置
        public float cost;//从开始到此地已经付出的代价
        public float expect;//从此地到目标地时，预计还需要的代价

        public PosInner(int rowAt, int columnAt, int moveDfct)
        {
            coord = new Vector2Int(columnAt, rowAt);
            this.moveDfct = moveDfct;
        }
    };

    PosInner begin;
    PosInner end;

    List<List<PosInner>> map;//生成一个数据地图，先填充完一行的每一列，再填下一行
    int numRow;//行数
    int numColumn;//列数

    LinkedList<PosInner> opens;
    //删除频繁，因此用链表结构

    List<PosInner> closes;
    //稳定增加，因此用数组结构
    //归入这个列表的，都是不会再影响路径结果的

    public List<Vector2Int> meShortestPath;//相对当前起点与终点的路径图，而且路径是最短路径

    public MatrixMapPath(List<List<int>> moveDfct, Vector2Int from, Vector2Int to)
    {
        //moveDfct遵循先行后列的填充方式，以移动难度的形式给予地图信息

        opens = new LinkedList<PosInner>();
        closes = new List<PosInner>();
        meShortestPath = new List<Vector2Int>();

        map = new List<List<PosInner>>();
        if (moveDfct.Count > 0 && moveDfct[0].Count > 0)
        {
            numRow = moveDfct.Count;
            numColumn = moveDfct[0].Count;//要求传过来一个矩形地图，所以任一行具有的列数应一样
            for (int i = 0; i < numRow; i++)
            {
                map.Add(new List<PosInner>());
                for (int j = 0; j < numColumn; j++)
                {
                    PosInner pos = new PosInner(i, j, moveDfct[i][j]);//创建一个位置点
                    map[i].Add(pos);//加入到地图数据中
                }
            }
            ResetBeginAndEnd(from, to);
        }

    }
    
    public void ResetBeginAndEnd(Vector2Int from, Vector2Int to)
    {
        begin = map[from[1]][from[0]];//x表达哪一列，y表达哪一行
        end = map[to[1]][to[0]];
        ClearFinding();//先重置用来寻路的相关数据
        FindShortestPath();
    }

    void ClearFinding()
    {
        opens.Clear();
        closes.Clear();
        meShortestPath.Clear();
        foreach (var aRow in map)
        {
            foreach (var pos in aRow)
            {
                pos.cost = int.MaxValue;
                pos.pre = null;
            }
        }
        begin.cost = 0;
    }

    //-----------------------------------------------------------
    
    bool FindShortestPath()
    {
        opens.AddLast(begin);//放入开启列表
        for (int acc = 0; acc < 1000; acc++)//容纳很多次，但不能一直都没有找到，性能压力会很大，无意义
        {
            if (opens.Count > 0)
            {
                PosInner finder = GetRecommendFromOpenWait();//从开启列表中提取出一个最佳元素
                //从开启列表中取得的最短路径者，就是到相应位置的最短路径了，没法再少
                opens.Remove(finder);//从开启列表中移除该元素
                closes.Add(finder);//放入关闭列表

                List<PosInner> nearOpens = GetNearIdle(finder);//加入周围元素到开启列表
                for (int i = 0; i < nearOpens.Count; i++)
                {
                    if (nearOpens[i] == end)//当含有终点元素时，路径就可以确定了
                    {
                        for (PosInner back = end; back != begin; back = back.pre)
                            meShortestPath.Add(back.coord);//延路径返回
                        meShortestPath.Add(begin.coord);//添上起始点
                        meShortestPath.Reverse();//倒序存储，方便外界读取
                        return true;
                    }
                    else
                        opens.AddLast(nearOpens[i]);
                }
            }
            else
            {
                Debug.Log("路径不存在");
                return false;
            }
        }
        Debug.Log("放弃寻找路径");
        return false;
    }
    
    PosInner GetRecommendFromOpenWait()
    {//取得相对当前情况来说，最佳的探索位置
        PosInner recommend = opens.First.Value;//从首个元素开始轮换
        float lowest = recommend.cost + recommend.expect;
        foreach (var round in opens)
        {
            float pay = round.cost + round.expect;
            if (pay < lowest)
            {
                recommend = round;
                lowest = pay;
            }
        }
        return recommend;
    }

    List<PosInner> GetNearIdle(PosInner centre)
    {
        List<PosInner> nears = new List<PosInner>();
        MatrixMap.GetAllNear4(centre.coord, numRow, numColumn, (one) =>
        {
            PosInner near = map[one.y][one.x];
            if (near.moveDfct != 0) //位置得是可走的
            {
                if (!closes.Contains(near))
                {//位置不能处于关闭列表中
                    //顺便做了一些必要的数据处理，标记从本地到邻地的代价
                    UpdateNeed(centre, near); 
                    //先看开启列表中是否已经含有，刚寻找到的相邻元素,
                    //已经含有指定元素，则不需要再加入，不算是闲置状态
                    if (!opens.Contains(near))
                        nears.Add(near);
                }
            }
        });
        return nears;
    }
    
    //-----------------------------------------------------------
    
    void UpdateNeed(PosInner cur, PosInner post)
    {
        post.expect = CountExpect(post);
        float thisCost = CountCost(cur, post);
        if (post.cost > thisCost)//当原来的代价大于当前路径代价时，替换为当前路径
        {
            post.pre = cur;
            post.cost = thisCost;
        }
    }

    float CountCost(PosInner mid, PosInner around)
    {
        //该函数认为已经保证所传入参数所代表的区域是相邻的
        //面向上下左右对齐的方格阵列来算的
        if (Mathf.Abs(around.coord.x - mid.coord.x) == 0)
            return mid.cost + 10;
        else if (Mathf.Abs(around.coord.y - mid.coord.y) == 0)
            return mid.cost + 10;
        else
            return mid.cost + 14;
    }
    
    float CountExpect(PosInner here)
    {
        int width = Mathf.Abs(end.coord.x - here.coord.x);
        int height = Mathf.Abs(end.coord.y - here.coord.y);
        return (width + height) * 10;
    }

};

/*测试脚本
 * public class Test : MonoBehaviour
{

    void Start()
    {
        //初始化地图，用二维矩阵代表地图，1表示障碍物，0表示可通
        List<List<int>> maze = new List<List<int>>() {
        new List<int>(){ 1,1,1,1,1,1},//注意这里才是行1，而不是最后一行是行1
        new List<int>(){ 1,1,0,0,1,1},
        new List<int>(){ 1,0,1,1,1,1},
        new List<int>(){ 0,1,1,0,1,1},
        new List<int>(){ 1,1,0,1,1,1},
        };
        MatrixMapPath map = new MatrixMapPath(maze, new Vector2Int(1, 0), new Vector2Int(1, 4));

        foreach (var one in map.meShortestPath)
            Debug.Log(one);

        //map.ResetBeginAndEnd(new Vector2Int(1, 4), new Vector2Int(1, 0));

        //foreach (var one in map.meShortestPath)
        //    Debug.Log(one);
    }
}

 */
