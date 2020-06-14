/*#******************************************************************************#*/
/*#    SettingManager                                                            #*/
/*#                                                                              #*/
/*#    Summary    :    SettingSceneの管理                                        #*/
/*#******************************************************************************#*/
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private InputField roomNameInputField = default;

    [SerializeField]
    private Dropdown membersCountDropdown = default;

    [SerializeField]
    private RoomTableOperator roomTableOperator = default;

    private GameObject roomTable;
    private List<RoomInfo> roomInfos = new List<RoomInfo>();

    public void Start()
    {
        //PhotonServerSettingに設定した内容を使ってマスターサーバに接続する
        PhotonNetwork.ConnectUsingSettings();

        roomTable = GameObject.Find("RoomTable");
        roomTable.SetActive(false);
    }

    /*#======================================================================#*/
    /*#    function : GetRoomInfos  function                                 #*/
    /*#    summary  : room情報を取得する                                     #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : (O) List<RoomInfo>  roomInfos   -  room情報            #*/
    /*#======================================================================#*/
    public List<RoomInfo> GetRoomInfos()
    {
        OnRoomListUpdate(roomInfos);
        return roomInfos;
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : SetMyNickName  function                                #*/
    /*#    summary  : ニックネームをつける                                   #*/
    /*#    argument : (I)string  nickName             -  ニックネーム        #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    private void SetMyNickName()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.NickName;
        }
    }

    private void joinLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    /// <summary>
    /// ホームボタンを押したときの処理。
    /// </summary>
    public void PushHomeButton()
    {
        //Photonロビーから出る
        PhotonNetwork.LeaveLobby();
        //Photonネットワークの接続を切る
        PhotonNetwork.Disconnect();
        //ホームへ
        GameManager.Instance.LoadGameScene("OpenningScene");
    }

    /// <summary>
    /// 作成して入室ボタンを押したときの処理
    /// </summary>
    public void PushEnterButton()
    {
        //ルームオプション設定
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = Convert.ToByte(membersCountDropdown.value + 1),
            PlayerTtl = 1000 * 60
            //MaxPlayers = Convert.ToByte(membersCountDropdown.value),
            //CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            //{
            //    {
            //        "CustomProperties", "カスタムプロパティ"
            //    }
            //},
            //CustomRoomPropertiesForLobby = new string[]
            //{
            //    "CustomProperties"
            //}
        };

        //設定された名前のルームを作成して参加する。
        PhotonNetwork.JoinOrCreateRoom(roomNameInputField.text, roomOptions, TypedLobby.Default);
    }

    /// <summary>
    /// ルームテーブル上の入室ボタンを押したときの処理。
    /// </summary>
    /// <param name="rowIndex"></param>
    public void PushEnterButtonInTable(int rowIndex)
    {
        //すでに作成済みのルームをテーブルから指定して参加する
        PhotonNetwork.JoinRoom(roomTableOperator.GetRoomNameByCellIndex(rowIndex));
    }

    /// <summary>
    /// 部屋を探すボタンを押したときの処理。
    /// </summary>
    public void PushSeekRoomButton()
    {
        roomTable.SetActive(true);
        roomTableOperator.InitRoomTable();
    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                         Pun Callback List                          ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /* Photonに接続したとき */
    public override void OnConnected()
    {
        Debug.Log("ネットワークに接続しました。");
        SetMyNickName();
        Debug.Log("ようこそ、" + PhotonNetwork.LocalPlayer.NickName + "さん。");
    }

    /* Photonから切断されたとき */
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("ネットワークから切断されました。");
    }

    /* マスターサーバへ接続したとき */
    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターサーバに接続しました。");

        //ロビーに入る
        joinLobby();
    }

    /* ロビーに入ったとき */
    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました。");
    }

    /* ロビーから出たとき */
    public override void OnLeftLobby()
    {
        Debug.Log("ロビーから出ました。");
    }

    /* 部屋を作成したとき */
    public override void OnCreatedRoom()
    {
        Debug.Log("部屋を作成しました。");
    }

    /* 部屋の作成に失敗したとき */
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("部屋の作成に失敗しました。");
    }

    /* 部屋に入室したとき */
    public override void OnJoinedRoom()
    {
        //遷移前のシーンでネットワークオブジェクトを生成しないようにする
        PhotonNetwork.IsMessageQueueRunning = false;
        //ルームに移動
        GameManager.Instance.LoadGameScene("LobbyScene");
        
        Debug.Log("部屋に入室しました。");
    }

    /* 特定の部屋への入室に失敗したとき */
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("部屋の入室に失敗しました。");
    }

    /* ランダムな部屋の入室に失敗したとき */
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ランダムな部屋の入室に失敗しました。");
    }

    /* 部屋から退室したとき */
    public override void OnLeftRoom()
    {
        Debug.Log("部屋から退室しました。");
    }

    /* 他のプレイヤーが入室したとき */
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("他のプレイヤーが入室してきました。");
    }

    /* 他のプレイヤーが退室したとき */
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("他のプレイヤーが退室しました。");
    }

    /* マスタークライアントが変わったとき */
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("マスタークライアントが変更されました。");
    }

    /* ロビーに更新があったとき */
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("ロビーが更新されました。");
    }

    /* ルームリストに更新があったとき */
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("ルームリストに更新がありました。");

        roomInfos = roomList;
    }

    /* ルームプロパティが更新されたとき */
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("ルームプロパティが更新されたとき");
    }

    /* プレイヤープロパティが更新されたとき */
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("プレイヤープロパティが更新されました。");
    }

    /* フレンドリストに更新があったとき */
    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        Debug.Log("フレンドリストに更新がありました。");
    }

    /* 地域リストを受け取ったとき */
    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("地域リストを取得しました。");
    }

    /* WebRpcのレスポンスがあったとき */
    //public override void OnWebRpcResponse(OperationResponse response)
    //{
    //    Debug.Log("WebRcpの応答を検出しました。");
    //}

    /* カスタム認証のレスポンスがあったとき */
    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        Debug.Log("カスタム認証の応答を検出しました。");
    }

    /* カスタム認証に失敗したとき */
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log("カスタム認証に失敗しました。");
    }
}
