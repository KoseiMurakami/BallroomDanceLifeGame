using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    GameObject startPoint = default;

    [SerializeField]
    GameObject playerPrefab = default;

    [SerializeField]
    int membersCount = 0;

    private List<Player> players = new List<Player>();
    private Vector3 instantiatePos;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < membersCount; i++)
        {
            instantiatePos = new Vector3(startPoint.transform.position.x,
                                         startPoint.transform.position.y,
                                         startPoint.transform.position.z);

            //playerをスタートポイントにインスタンス化
            GameObject playerObject = Instantiate(playerPrefab,
                                                  instantiatePos,
                                                  transform.rotation) as GameObject;

            Player player = playerObject.GetComponent<Player>();
            players.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*#======================================================================#*/
    /*#    function : DeliveryDiceTheEyes  function                          #*/
    /*#    summary  : 出たさいころの目をアクティブプレイヤーに引き渡す       #*/
    /*#    argument : int            index            -  マスインデックス    #*/
    /*#               squareKindDef  kind             -  マス種別            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void DeliveryDiceTheEyes(int eyes)
    {
        players[0].GetDiceEyes(eyes);
    }
}
