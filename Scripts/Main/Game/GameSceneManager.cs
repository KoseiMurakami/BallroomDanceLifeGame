//using Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    [SerializeField]
    GameObject startPoint = default;

    [SerializeField]
    private Text diceEyesText = default;

    [SerializeField]
    GameObject eventTextPanel = default;

    [SerializeField]
    MainCamera mainCamera = default;

    [SerializeField]
    private GameObject canvas = default;

    private PunTurnManager turnManager;
    private int turn = 0;
    private GameObject playerPrefab;
    private GameObject[] playerStatusPanelPrefab;
    private GameObject playerObject;
    private List<GameObject> playerObjectList = new List<GameObject>();
    private GamePlayer gamePlayer;
    private List<GamePlayer> gamePlayerList = new List<GamePlayer>();
    private int activePlayerIndex;
    private GameObject activePlayerObject;
    private Vector3 instantiatePos;

    private void Awake()
    {
        turnManager = GetComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        turnManager.TurnDuration = 30f;

        //turnManagerを起動するのに必須
        PhotonNetwork.IsMessageQueueRunning = true;

        playerPrefab = Resources.Load<GameObject>("Prefabs/NetworkObjects/GamePlayer");
        playerStatusPanelPrefab = Resources.LoadAll<GameObject>("Prefabs/UI");

        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            //プレイヤー生成の起点
            instantiatePos = new Vector3(startPoint.transform.position.x + Random.Range(-2, 2),
                                         startPoint.transform.position.y,
                                         startPoint.transform.position.z + Random.Range(-2, 2));

            //プレイヤー生成＆情報キャッシュ
            playerObject = Instantiate(playerPrefab, instantiatePos, Quaternion.identity);
            playerObjectList.Add(playerObject);
            gamePlayer = playerObject.GetComponent<GamePlayer>();
            gamePlayerList.Add(gamePlayer);

            //ステータスパネル人数分生成(親要素をcanvasに設定しないと表示されない)
            GameObject panel = Instantiate(playerStatusPanelPrefab[i]);
            panel.transform.SetParent(canvas.transform, false);

            //プレイヤー情報を設定
            gamePlayer.PlayerId = i;
            gamePlayer.PlayerName = PhotonNetwork.CurrentRoom.Players[i + 1].NickName;
        }
    }

    /*#======================================================================#*/
    /*#    function : DisplayEventTextPanel  function                        #*/
    /*#    summary  : eventTextPanelを表示する                               #*/
    /*#    argument : string         text             -  表示テキスト        #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void DisplayEventTextPanel(string text)
    {
        eventTextPanel.SetActive(true);
        Text displayText = eventTextPanel.transform.Find("EventText").GetComponent<Text>();

        displayText.text = text;
    }

    /// <summary>
    /// アクティブプレイヤーインデックスをセットする
    /// </summary>
    private void SetActivePlayerIndex()
    {
        activePlayerIndex = turnManager.Turn % PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    /// <summary>
    /// アクティブプレイヤーオブジェクトをセットする
    /// </summary>
    private void SetActivePlayerObject()
    {
        activePlayerObject = playerObjectList[activePlayerIndex];
    }

    /*#======================================================================#*/
    /*#    function : RequireActStart  function                              #*/
    /*#    summary  : 行動開始要求を受け取る                                 #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void RequireActStart()
    {
        //マスタークライアントがターンを開始する
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("マスタークライアントがターンを開始しました。");
            turnManager.BeginTurn();
        }
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : isThisMyTurn  function                                 #*/
    /*#    summary  : 現在のターンが自分のターンか判別する                   #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : bool  true                      -  自分のターン        #*/
    /*#                     false                     -  自分以外のターン    #*/
    /*#----------------------------------------------------------------------#*/
    private bool isThisMyTurn()
    {
        string nowPlayerId = PhotonNetwork.CurrentRoom.Players[activePlayerIndex + 1].UserId;

        if (nowPlayerId == PhotonNetwork.LocalPlayer.UserId)
        {
            return true;
        }

        return false;
    }

    /*#======================================================================#*/
    /*#    function : RollTheDice  function                                  #*/
    /*#    summary  : さいころをふる                                         #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void RollTheDice()
    {
        if (isThisMyTurn())
        {
            int eyes;
            eyes = Random.Range(1, 11);

            ////プレイヤーカスタムプロパティの更新
            var hashtable = new ExitGames.Client.Photon.Hashtable()
            {
                ["DiceEyes"] = eyes
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    /// <summary>
    /// マスの残り数を表示する
    /// </summary>
    /// <param name="eyes"></param>
    public void DisplayRestDiceEyes(int eyes)
    {
        diceEyesText.text = eyes.ToString();
    }

    /// <summary>
    /// TextButtonを押したときの処理
    /// </summary>
    public void PushTextButton()
    {
        eventTextPanel.SetActive(false);
        turnManager.SendMove(0, true);
    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                     TurnManager Callback List                      ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    //ターンが開始したとき
    public void OnTurnBegins(int turn)
    {
        Debug.Log(turn + "ターン目が開始しました。");
        SetActivePlayerIndex();
        SetActivePlayerObject();
        mainCamera.SetTargetObj(activePlayerObject);

        if (isThisMyTurn())
        {
            Debug.Log("自分のターンです。");
            //自分のターン処理
            gamePlayerList[activePlayerIndex].SwitchDiceButtonDisplay(true);
        }
        else
        {
            Debug.Log("相手のターンです。");
            //自分以外のターン処理
            turnManager.SendMove(0, true);
        }
    }

    //ターンのムーブが全プレイヤー完了したとき
    public void OnTurnCompleted(int turn)
    {
        Debug.Log("ターンが終了しました。");
        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.BeginTurn();
        }
    }

    //プレイヤーのムーブを開始したとき
    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("プレイヤーのムーブが開始しました。");
    }

    //プレイヤーのムーブが終了したとき
    public void OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("プレイヤーのムーブが終了しました。");
    }

    //タイマーがタイムアウトしたときに処理したい内容を書く
    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("ターンがタイムアウトしました。");
    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                         Pun Callback List                          ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    //カスタムプロパティの値が変更されたとき
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //変更されたキーと値のペア出力
        foreach (var p in changedProps)
        {
            Debug.Log($"{p.Key}: {p.Value}");
        }

        int eyes = (int)changedProps["DiceEyes"];

        diceEyesText.text = eyes.ToString();
        gamePlayerList[activePlayerIndex].setTheDice(eyes);
    }
}
