using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LifeGame;

public class Square : SquareBase
{
    private Vector3 nextPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*#======================================================================#*/
    /*#    function : SetSquareInfo  function                                #*/
    /*#    summary  : マスの情報をセットする                                 #*/
    /*#    argument : int            index            -  マスインデックス    #*/
    /*#               squareKindDef  kind             -  マス種別            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SetSquareInfo(int index, squareKindDef kind)
    {
        squareIndex = index;
        squareKind = kind;
    }

    /*#======================================================================#*/
    /*#    function : SetNextSquarePos  function  　                         #*/
    /*#    summary  : 次のマス位置をセットする                               #*/
    /*#    argument : int            index            -  マスインデックス    #*/
    /*#               squareKindDef  kind             -  マス種別            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SetNextSquarePos(Vector3 pos)
    {
        nextPos = pos;
    }

    /*#======================================================================#*/
    /*#    function : GetNextSquarePos  function                                   #*/
    /*#    summary  : 次のマスの位置を取得する                               #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : int  nextIndex              -  次のマスインデックス    #*/
    /*#======================================================================#*/
    public Vector3 GetNextSquarePos()
    {
        return nextPos;
    }
}
