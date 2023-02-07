using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAssist
{
    public static Vector2 GetSizeCover(Transform one)
    {
        Vector2 sizeSprite = SpriteAssist.GetSizeInScene(one.GetComponent<SpriteRenderer>().sprite);
        Vector2 scale = GbjAssist.GetSumScaleWhenSelf(one);
        return sizeSprite * scale;
    }

    

}
