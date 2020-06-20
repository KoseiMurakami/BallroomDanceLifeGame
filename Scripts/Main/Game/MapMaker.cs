using LifeGame;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    private MasterDataRepository masterDataRepository;
    private readonly List<Square> squareList = new List<Square>();
    private List<SquareInfo> squareInfList = new List<SquareInfo>();
    private readonly Dictionary<int, GameObject> squareObjDic = new Dictionary<int, GameObject>();

    void Start()
    {
        int roopIndex = 0;
        masterDataRepository = Resources.Load<MasterDataRepository>("MasterData/MasterDataRepository");
        masterDataRepository.GetSquareInfList(ref squareInfList);

        //子オブジェクトをすべて取得し、コンポーネントを追加、設定
        foreach (Transform childTransform in gameObject.transform)
        {
            //子オブジェクトにSquareコンポーネントを追加
            Square square = childTransform.gameObject.AddComponent<Square>();
            squareList.Add(square);
            square.SetSquareInfo(squareInfList[roopIndex]);

            //マスIDをキーにしてオブジェクト情報を追加
            squareObjDic.Add(squareInfList[roopIndex].id, childTransform.gameObject);

            //マス種別に応じてマスの色を設定
            switch (squareInfList[roopIndex].squareKind)
            {
                case SquareKindDef.forced:
                    childTransform.GetComponent<Renderer>().material.color = Color.white;
                    break;
                case SquareKindDef.brotherShop:
                case SquareKindDef.partnerShop:
                case SquareKindDef.dualShop:
                    childTransform.GetComponent<Renderer>().material.color = Color.green;
                    break;
                case SquareKindDef.steal:
                    childTransform.GetComponent<Renderer>().material.color = Color.black;
                    break;
                case SquareKindDef.warp:
                    childTransform.GetComponent<Renderer>().material.color = Color.red;
                    break;
            }

            roopIndex++;
        }

        //次マス位置とワープ先マス位置をセット
        for (int i = 0; i < squareObjDic.Count; i++)
        {
            if (squareInfList[i].nextId != 0)
            {
                squareList[i].NextPos = squareObjDic[squareInfList[i].nextId].transform.position;
            }
            if (squareInfList[i].warpId != 0)
            {
                squareList[i].WarpPos = squareObjDic[squareInfList[i].warpId].transform.position;
            }
        }
    }
}
