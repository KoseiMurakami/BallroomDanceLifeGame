/*#==========================================================================#*/
/*#    MasterDataRepository                                                  #*/
/*#                                                                          #*/
/*#    Summary    :    マスターデータの配置とロード                          #*/
/*#                                                                          #*/
/*#==========================================================================#*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MasterDataRepository : ScriptableObject
{
    [SerializeField] MstSquares mstSquares = default;

    /*#======================================================================#*/
    /*#    function : GetSquareInfList  function                             #*/
    /*#    summary  : マス情報リストを取得する                               #*/
    /*#    argument : (O)                                                    #*/
    /*#    return   :                                                        #*/
    /*#======================================================================#*/
    public void GetSquareInfList(ref List<MstSquaresDef> squareInfList)
    {
        squareInfList = mstSquares.entities;
    }
}