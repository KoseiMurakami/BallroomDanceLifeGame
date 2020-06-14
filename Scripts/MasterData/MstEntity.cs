using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LifeGame;

[System.Serializable]
public class MstSquaresDefForExcel
{
    public int id;                                    /* マスID             */
    public SquareKindDef squareKind;                  /* マス種別           */
    public int nextId;                                /* 次マスID           */
    public string pointKind;                          /* ポイント種別       */
    public string points;                             /* ポイント           */
    public LankKindDef lankKind;                      /* ランク種別         */
    public int lankPoints;                            /* ランクポイント     */
    public int warpId;                                /* ワープ先ID         */
    public string eventText;                          /* マス停止時テキスト */
}

public class SquareInfo
{
    public int id;                                    /* マスID             */
    public SquareKindDef squareKind;                  /* マス種別           */
    public int nextId;                                /* 次マスID           */
    public List<PointKindDef> pointKindList           /* ポイント種別リスト */
        = new List<PointKindDef>();                   
    public List<int> pointList = new List<int>();     /* ポイントリスト     */
    public LankKindDef lankKind;                      /* ランク種別         */
    public int lankPoint;                             /* ランクポイント     */
    public int warpId;                                /* ワープ先ID         */
    public string eventText;                          /* マス停止時テキスト */
}