using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UIElements;

namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class ResourceManager : MonoBehaviour
    {
        //オブジェクトプール
        private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

        private static ResourceManager mInstance = null;

        /* ResourceManager.Instanceで呼び出すことができる */
        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("ResourceManager", typeof(ResourceManager));
                    go.transform.localPosition = new Vector3(9999999, 9999999, 9999999);
                    /* Kanglai : `GO.hideFlags |= HideFlags.DontSave;`ならば、PlayingモードをやめるときDestroyの問題がある。*/
                    /* しかし、Play mode のときは使い続けなければならない。*/
                    mInstance = go.GetComponent<ResourceManager>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("[ResourceManager] Editor modeのときはResourceManagerを無視してください。");
                    }
                }
                return mInstance;
            }
        }

        /*#======================================================================#*/
        /*#    function : InitPool  function                                     #*/
        /*#    summary  : プールのイニシャライズ                                 #*/
        /*#    argument : string  poolName  -  プール名                          #*/
        /*#               int     size      -  プールのサイズ                    #*/
        /*#               PoolInflationType  type                                #*/
        /*#                                 -  プールタイプ                      #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        public void InitPool(string poolName, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                GameObject pb = Resources.Load<GameObject>(poolName);
                if (pb == null)
                {
                    Debug.LogError("[ResourceManager] " + poolName + "をResourcesフォルダに追加してください。");
                }
                poolDict[poolName] = new Pool(poolName, pb, gameObject, size, type);
            }
        }

        /*#======================================================================#*/
        /*#    function : GetObjectFromPool  function                            #*/
        /*#    summary  : プールのイニシャライズ                                 #*/
        /*#    argument : string  poolName  -  プール名                          #*/
        /*#               int     size      -  プールのサイズ                    #*/
        /*#               PoolInflationType  type                                #*/
        /*#                                 -  プールタイプ                      #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        public GameObject GetObjectFromPool(string poolName, bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(poolName) && autoCreate > 0)
            {
                InitPool(poolName, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                Pool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //利用可能オブジェクトがプールにあるときのシナリオ
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:" + poolName + "には利用可能オブジェクトがありません。");
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:指定された" + poolName + "は無効です。");
            }
#endif
            return result;
        }

        /*#======================================================================#*/
        /*#    function : ReturnObjectToPool  function                           #*/
        /*#    summary  : プールにオブジェクトを返す                             #*/
        /*#    argument : GameObject  go    -  ゲームオブジェクト                #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        public void ReturnObjectToPool(GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning(go.name + "で指定されたオブジェクトはプールされたインスタンスではありません。");
#endif
            }
            else
            {
                Pool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("利用可能な" + po.poolName + "という名のプールはありません。");
                }
#endif
            }
        }

        /*#======================================================================#*/
        /*#    function : ReturnTransformToPool  function                        #*/
        /*#    summary  : プールにトランスフォームを返す                         #*/
        /*#    argument : Transform  t      -  トランスフォーム                  #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        public void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] プールにnull transformを返そうとしています。");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }

       
    }
}