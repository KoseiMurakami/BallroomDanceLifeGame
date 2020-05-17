using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject startPoint = default;

    [SerializeField]
    GameObject eventTextPanel = default;

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

    /*#======================================================================#*/
    /*#    function : DisplayEventTextPanel  function                        #*/
    /*#    summary  : 3秒間の間eventTextPanelを表示する                      #*/
    /*#    argument : string         text             -  表示テキスト        #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void DisplayEventTextPanel(string text)
    {
        eventTextPanel.SetActive(true);
        Text displayText = eventTextPanel.transform.Find("EventText").GetComponent<Text>();

        displayText.text = text;

        StartCoroutine("eventTextPanelKeep");
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : eventTextPanelKeep  collider                           #*/
    /*#    summary  : eventTextPanelを3秒間キープする                        #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    private IEnumerator eventTextPanelKeep()
    {
        yield return new WaitForSeconds(3);

        eventTextPanel.SetActive(false);

        yield break;
    }
}
