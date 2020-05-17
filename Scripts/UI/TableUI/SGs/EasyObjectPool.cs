/*
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
// modified version by Kanglai Qian
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolObject : MonoBehaviour
    {
        public string poolName;
        public bool isPooled;  //オブジェクトがプール中か表示中かのフラグ
    }

    public enum PoolInflationType
    {
        /// When a dynamic pool inflates, one to the pool
        INCREMENT,
        /// When a dynemic pool inflates, double the size of the pool
        DOUBLE
    }

    class Pool
    {
        private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

        //ルートオブジェクトの定義
        private GameObject rootObj;
        private PoolInflationType inflationType;
        private string poolName;
        private int objectsInUse = 0;

        //コンストラクタ
        public Pool(string poolName, GameObject poolObjectPrefab, GameObject rootPoolObj, int initialCount, PoolInflationType type)
        {
            if (poolObjectPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ObjPoolManager] null pool object prefab !");
#endif
                return;
            }
            this.poolName = poolName;
            this.inflationType = type;
            this.rootObj = new GameObject(poolName + "Pool");
            this.rootObj.transform.SetParent(rootPoolObj.transform, false);

            // In case the origin one is Destroyed, we should keep at least one
            GameObject go = GameObject.Instantiate(poolObjectPrefab);
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                po = go.AddComponent<PoolObject>();
            }
            po.poolName = poolName;
            AddObjectToPool(po);

            //プール領域へ投入
            populatePool(Mathf.Max(initialCount, 1));
        }

        private void AddObjectToPool(PoolObject po)
        {
            //プール領域へ加える
            po.gameObject.SetActive(false);
            po.gameObject.name = poolName;
            availableObjStack.Push(po);
            po.isPooled = true;
            //ルートオブジェクトへ加える
            po.gameObject.transform.SetParent(rootObj.transform, false);
        }

        private void populatePool(int initialCount)
        {
            for (int index = 0; index < initialCount; index++)
            {
                PoolObject po = GameObject.Instantiate(availableObjStack.Peek());
                AddObjectToPool(po);
            }
        }

        public GameObject NextAvailableObject(bool autoActive)
        {
            PoolObject po = null;
            if (availableObjStack.Count > 1)
            {
                po = availableObjStack.Pop();
            }
            else
            {
                int increaseSize = 0;
                if (inflationType == PoolInflationType.INCREMENT)
                {
                    increaseSize = 1;
                }
                else if (inflationType == PoolInflationType.DOUBLE)
                {
                    increaseSize = availableObjStack.Count + Mathf.Max(objectsInUse, 0);
                }
                if (increaseSize > 0)
                {
                    populatePool(increaseSize);
                    po = availableObjStack.Pop();
                }
            }

            GameObject result = null;
            if (po != null)
            {
                objectsInUse++;
                po.isPooled = false;
                result = po.gameObject;
                if (autoActive)
                {
                    result.SetActive(true);
                }
            }

            return result;
        }

        public void ReturnObjectToPool(PoolObject po)
        {
            if (poolName.Equals(po.poolName))
            {
                objectsInUse--;
                /* もしオブジェクトがプールにあればavailableObjStack.Contains(po)メソッドを使ってチェックができない。*/
                /* ここではより堅牢なコードにするために以下のメソッドを用意した。                                    */
                if (po.isPooled)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(po.gameObject.name + "はすでにプール中です。使用法を確認し、もう一度試してみてください。");
#endif
                }
                else
                {
                    AddObjectToPool(po);
                }
            }
            else
            {
                Debug.LogError(string.Format("間違ったプール {0} {1} へオブジェクトを追加しようとしています。", po.poolName, poolName));
            }
        }
    }
}
