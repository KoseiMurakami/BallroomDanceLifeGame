using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LifeGame;

public class Square : SquareBase
{
    private SquareInfo squareInfo = new SquareInfo();
    public bool IsForced { set; get; }
    public Vector3 NextPos { set; get; }
    public Vector3 WarpPos { set; get; }

    /*#======================================================================#*/
    /*#    function : SetSquareInfo  function                                #*/
    /*#    summary  : マスの情報をセットする                                 #*/
    /*#    argument : SquareInfo  data                -  マス情報            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SetSquareInfo(SquareInfo data)
    {
        squareInfo = data;
        if (data.squareKind == SquareKindDef.forced)
        {
            IsForced = true;
        }
        else
        {
            IsForced = false;
        }
    }

    /// <summary>
    /// マス情報を取得する
    /// </summary>
    /// <returns>マス情報</returns>
    public SquareInfo GetSquareInfo()
    {
        return squareInfo;
    }
}
