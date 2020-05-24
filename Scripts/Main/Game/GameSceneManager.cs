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

    [SerializeField]
    Game_Button game_Button = default;

    [SerializeField]
    MainCamera mainCamera = default;

    private int activePlayerIndex = 0;
    private int myPlayerIndex = 0;
    private int exsistsPlayerCount;
    private GameObject playerObject;
    private List<GameObject> playerObjectList = new List<GameObject>();
    private Player player;
    private List<Player> playerList = new List<Player>();
    private Vector3 instantiatePos;
    private bool isCalledOnce = true;

    private void Awake()
    {
        //プレイヤー生成の起点
        instantiatePos = new Vector3(startPoint.transform.position.x,
                                     startPoint.transform.position.y,
                                     startPoint.transform.position.z);

        //プレイヤー生成
        playerObject = PhotonNetwork.Instantiate("Prefabs/NetworkObjects/GamePlayer",
                                                 instantiatePos,
                                                 Quaternion.identity);

        player = playerObject.AddComponent<Player>();
        playerObject.name = PhotonNetwork.LocalPlayer.UserId;
        PhotonNetwork.IsMessageQueueRunning = true;
        exsistsPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Start()
    {
        for (int i = 1; i <= exsistsPlayerCount; i++)
        {
            playerObject = GameObject.Find(PhotonNetwork.CurrentRoom.Players[i].UserId);
            player = playerObject.GetComponent<Player>();
            playerObjectList.Add(playerObject);
            playerList.Add(player);

            //自身のプレイヤーインデックスを記憶しておく
            if (PhotonNetwork.CurrentRoom.Players[i].UserId == PhotonNetwork.LocalPlayer.UserId)
            {
                myPlayerIndex = i - 1;
            }
        }
        

        game_Button.SwitchButtonDisplay(false);
    }

    private void Update()
    {
        mainCamera.SetTargetObj(playerObjectList[activePlayerIndex]);

        //ボタンの初期化処理
        if (myPlayerIndex == activePlayerIndex)
        {
            if (isCalledOnce)
            {
                game_Button.SwitchButtonDisplay(true);
                isCalledOnce = false;
            }
        }
        else
        {
            game_Button.SwitchButtonDisplay(false);
        }
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
        playerList[activePlayerIndex].GetDiceEyes(eyes);
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

    /*#======================================================================#*/
    /*#    function : GetPlayerList  function                                #*/
    /*#    summary  : プレイヤーリストを取得する                             #*/
    /*#    argument : (I)List<Player>  playerList     -  プレイヤーリスト    #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void GetPlayerList(ref List<Player> playerList)
    {
        playerList = this.playerList;
    }

    /*#======================================================================#*/
    /*#    function : GetPlayerObjectList  function                          #*/
    /*#    summary  : プレイヤーオブジェクト情報リストを取得する             #*/
    /*#    argument : (I)List<GameObject>  playerObjectList                  #*/
    /*#                                              -  プレイヤー情報リスト #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void GetPlayerObjectList(ref List<GameObject> playerObjectList)
    {
        playerObjectList = this.playerObjectList;
    }

    /*#======================================================================#*/
    /*#    function : ReplyActEnd  function                                  #*/
    /*#    summary  : 行動終了報告を受け取る                                 #*/
    /*#    argument : (I)List<GameObject>  playerObjectList                  #*/
    /*#                                              -  プレイヤー情報リスト #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void ReplyActEnd()
    {
        activePlayerIndex++;

        if (exsistsPlayerCount == activePlayerIndex)
        {
            activePlayerIndex = 0;
        }

        isCalledOnce = true;
    }
}
