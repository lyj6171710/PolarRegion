using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TilesetSelect : MonoBehaviour
{
    //-----------------------------------

    [HideInEditorMode]
    [ValueDropdown("GetFamiliesCanSelect")]
    public GameObject mCurSelectFamilyDp;

    [HideInEditorMode]
    [ValueDropdown("GetSetsCanSelect")]
    [OnValueChanged("SwitchSet")]
    public GameObject mCurSelectSetDp;

    [Required]
    [HideInPlayMode]
    public TilesetOneSet mSelectStartDp;

    List<GameObject> GetSetsCanSelect() 
    {
        if (mCurSelectFamilyDp != null)
            return mCurSelectFamilyDp.GetComponent<TilesetOneFamily>().SuGetSets();
        return null;
    }

    List<GameObject> GetFamiliesCanSelect()
    {
        if (MapPlates.It != null)
            return MapPlates.It.SuGetFamilies();
        return null;
    }

    void SwitchSet()
    {
        TilesetOneSet recent = mCurSelectSetDp.GetComponent<TilesetOneSet>();
        if (recent != null && recent != mCurApplySet)
        {
            mPalette.MakeDown();
            mCurApplySet = recent;
            mPalette.MakeUse(mCurApplySet);
        }
    }

    //-----------------------------------

    

    //=====================================

    MapPalette mPalette;
    TilesetOneSet mCurApplySet;
    TileSetUp mSetup;

    void Start()
    {
        mCurApplySet = mSelectStartDp;
        mCurSelectFamilyDp = mCurApplySet.transform.parent.gameObject;
        mCurSelectSetDp = mCurApplySet.gameObject;

        mPalette = GetComponent<MapPalette>();
        mPalette.MakeReady();
        mPalette.MakeUse(MapPlates.It.SuGetSet(mCurApplySet.meFamily, mCurApplySet.meName));

        mSetup = GetComponent<TileSetUp>();
        mSetup.MakeReady();
        mSetup.MakeUse();
    }


}
