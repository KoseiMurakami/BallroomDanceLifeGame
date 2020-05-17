using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    /* 抽象クラス */
    public abstract class LoopScrollDataSource
    {
        /* 抽象メソッド */
        public abstract void ProvideData(Transform transform, int idx);
    }

    public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
        public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

        LoopScrollSendIndexSource() { }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellIndex", idx);
        }
    }

    public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

        public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellContent", objectsToFill[idx]);
        }
    }
}