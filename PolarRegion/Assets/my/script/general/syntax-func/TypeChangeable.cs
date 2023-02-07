using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChangeable {
    

}

public class TypeCgeableOnBasis : TypeChangeable
{
    //虽然并没有节省多少空间占用，但是增加了安全性，封闭性

    bool vB;
    string vS;
    int vI;
    float vF;

    enum KindSelect { Bool,String,Int,Float};
    KindSelect now;

    public TypeCgeableOnBasis() { now = KindSelect.Bool; }

    //==================================

    //外界通过"&="来给该类对象赋值

    public static TypeCgeableOnBasis operator &(TypeCgeableOnBasis self, bool set)
    {
        self.vB = set;
        self.now = KindSelect.Bool;
        return self;
    }

    public static TypeCgeableOnBasis operator &(TypeCgeableOnBasis self, string set)
    {
        self.vS = set;
        self.now = KindSelect.String;
        return self;
    }

    public static TypeCgeableOnBasis operator &(TypeCgeableOnBasis self, int set)
    {
        self.vI = set;
        self.now = KindSelect.Int;
        return self;
    }

    public static TypeCgeableOnBasis operator &(TypeCgeableOnBasis self, float set)
    {
        self.vF = set;
        self.now = KindSelect.Float;
        return self;
    }

    //==================================

    public static bool operator ==(TypeCgeableOnBasis self, bool set)
    {
        if (self.now == KindSelect.Bool && self.vB == set) return true; else return false;
    }

    public static bool operator !=(TypeCgeableOnBasis self, bool set)
    {
        return !(self == set);
    }

    public static bool operator ==(TypeCgeableOnBasis self, string set)
    {
        if (self.now == KindSelect.String && self.vS == set) return true; else return false;
    }

    public static bool operator !=(TypeCgeableOnBasis self, string set)
    {
        return !(self == set);
    }

    public static bool operator ==(TypeCgeableOnBasis self, int set)
    {
        if (self.now == KindSelect.Int && self.vI == set) return true; else return false;
    }

    public static bool operator !=(TypeCgeableOnBasis self, int set)
    {
        return !(self == set);
    }

    public static bool operator ==(TypeCgeableOnBasis self, float set)
    {
        if (self.now == KindSelect.Float && self.vF == set) return true; else return false;
    }

    public static bool operator !=(TypeCgeableOnBasis self, float set)
    {
        return !(self == set);
    }

    //==================================
}

public class Tset
{
    TypeCgeableOnBasis var;

    public void t()
    {
        var = new TypeCgeableOnBasis();
        var &=  1;
        if (var == 1) return;
    }
}