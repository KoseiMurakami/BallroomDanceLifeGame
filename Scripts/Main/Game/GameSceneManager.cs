using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject startPoint = default;

    private GameObject playerObject;
    private Player player;
    private Vector3 instantiatePos;

    // Start is called before the first frame update
    void Start()
    {
        instantiatePos = new Vector3(startPoint.transform.position.x,
                                     startPoint.transform.position.y,
                                     startPoint.transform.position.z);

        playerObject = PhotonNetwork.Instantiate("Prefabs/NetworkObjects/GamePlayer", instantiatePos, Quaternion.identity);
        player = playerObject.GetComponent<Player>();
        PhotonNetwork.IsMessageQueueRunning = true;
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
        player.GetDiceEyes(eyes);
    }
}
