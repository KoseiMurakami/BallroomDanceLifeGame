using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LifeGame
{
    public enum SquareKindDef 
    {
        normal,
        forced
    }

    public class SquareBase : MonoBehaviour
    {

        protected int squareIndex;
        protected SquareKindDef squareKind;

        protected virtual void Start()
        {

        }

        
    }

    public class PlayerBase
    {
        private bool isMan;
        private int restCount;
        private Vector3 nextPosition;

        /*#======================================================================#*/
        /*#    function : GetNextPosition  function                              #*/
        /*#    summary  : 次に進むべき位置を取得する                             #*/
        /*#    argument : nothing                                                #*/
        /*#    return   : int  nextIndex              -  次のマスインデックス    #*/
        /*#======================================================================#*/

    }
}