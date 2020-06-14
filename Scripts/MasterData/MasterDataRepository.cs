/*#==========================================================================#*/
/*#    MasterDataRepository                                                  #*/
/*#                                                                          #*/
/*#    Summary    :    マスターデータの配置とロード                          #*/
/*#                                                                          #*/
/*#==========================================================================#*/
using System;
using System.Collections.Generic;
using UnityEngine;
using LifeGame;

[CreateAssetMenu]
public class MasterDataRepository : ScriptableObject
{
    [SerializeField] MstSquares mstSquares = default;

    /// <summary>
    /// マス情報をMstSquaresDefForExcel型からSquareInfo型に変換して取得する
    /// </summary>
    /// <param name="squareInfList">マス情報リスト</param>
    public void GetSquareInfList(ref List<SquareInfo> squareInfList)
    {
        for (int i = 0; i < mstSquares.entities.Count; i++)
        {
            SquareInfo squareInfo = new SquareInfo();

            squareInfo.id = mstSquares.entities[i].id;
            squareInfo.squareKind = mstSquares.entities[i].squareKind;
            squareInfo.nextId = mstSquares.entities[i].nextId;
            string[] pointKindArray = mstSquares.entities[i].pointKind.Split(',');
            for (int j = 0; j < pointKindArray.Length; j++)
            {
                PointKindDef pointKind;
                EnumExt.TryParse(pointKindArray[j], out pointKind);
                squareInfo.pointKindList.Add(pointKind);
            }
            string[] pointsArray = mstSquares.entities[i].points.Split(',');
            for (int j = 0; j < pointsArray.Length; j++)
            {
                int points = 0;
                if (pointsArray[j] != "")
                {
                    points = int.Parse(pointsArray[j]);
                }
                squareInfo.pointList.Add(points);
            }
            squareInfo.lankKind = mstSquares.entities[i].lankKind;
            squareInfo.lankPoint = mstSquares.entities[i].lankPoints;
            squareInfo.warpId = mstSquares.entities[i].warpId;
            squareInfo.eventText = mstSquares.entities[i].eventText;

            squareInfList.Add(squareInfo);
        }
    }

    /// <summary>
    /// stringからenumへの変換クラス
    /// </summary>
    static class EnumExt
    {
        internal static bool TryParse<TEnum>(string s, out TEnum value) where TEnum : struct
        {
            return Enum.TryParse(s, out value) && Enum.IsDefined(typeof(TEnum), value);
        }
    }
}