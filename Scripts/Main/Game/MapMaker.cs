using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    private MasterDataRepository masterDataRepository;
    private List<GameObject> squareObjectList = new List<GameObject>();
    private List<Square> squareList = new List<Square>();
    private List<MstSquaresDef> squareInfList;

    void Start()
    {
        int roopIndex = 0;
        masterDataRepository = Resources.Load<MasterDataRepository>("MasterData/MasterDataRepository");
        squareInfList = new List<MstSquaresDef>();
        masterDataRepository.GetSquareInfList(ref squareInfList);

        //子オブジェクトをすべて取得し、コンポーネントを追加、設定
        foreach (Transform childTransform in gameObject.transform)
        {
            //子オブジェクトにSquareコンポーネントを追加
            childTransform.gameObject.AddComponent<Square>();
            Square square = childTransform.gameObject.GetComponent<Square>();
            squareList.Add(square);
            square.SetSquareInfo(squareInfList[roopIndex].id, squareInfList[roopIndex].squareKind);
            squareObjectList.Add(childTransform.gameObject);
            roopIndex++;
        }

        for (int i = 0; i < squareObjectList.Count; i++)
        {
            if (i == squareObjectList.Count - 1)
            {
                squareList[i].SetNextSquarePos(squareObjectList[1].transform.position);
            }
            else
            {
                squareList[i].SetNextSquarePos(squareObjectList[i + 1].transform.position);
            }
        }
    }

    /*#======================================================================#*/
    /*#    function : GetSquarePosByIndex  function                          #*/
    /*#    summary  : 指定インデックスからマス位置を取得する                 #*/
    /*#    argument : (O)                                                    #*/
    /*#    return   :                                                        #*/
    /*#======================================================================#*/
    //public Vector3 GetSquarePosByIndex(int index)
    //{
    //    //squareInfList.Find
    //    //squareList.Find()
    //    //return squareInfList[index];
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
