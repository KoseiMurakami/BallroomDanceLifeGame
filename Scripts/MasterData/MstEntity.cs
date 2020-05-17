using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LifeGame;

[System.Serializable]
public class MstSquaresDef
{
    public int id;                                    /* マスID             */
    public SquareKindDef squareKind;                  /* マス種別           */
    public int nextId;                                /* 次マスID           */
    public string pointKind;                          /* ポイント種別       */
    public string points;                             /* ポイント           */
    public string lankKind;                           /* ランク種別         */
    public int lankPoints;                            /* ランクポイント     */
    public int warpId;                                /* ワープ先ID         */
    public string eventText;                          /* マス停止時テキスト */
}