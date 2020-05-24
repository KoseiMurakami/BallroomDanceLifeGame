/*#******************************************************************************#*/
/*#    SettingManager                                                            #*/
/*#                                                                              #*/
/*#    Summary    :    SettingSceneの管理                                        #*/
/*#******************************************************************************#*/
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingSceneManager : MonoBehaviourPunCallbacks
{
    private GameObject roomTable;
    private List<RoomInfo> roomInfos = new List<RoomInfo>();

    public void Start()
    {
        //PhotonServerSettingに設定した内容を使ってマスターサーバに接続する
        PhotonNetwork.ConnectUsingSettings();

        roomTable = GameObject.Find("RoomTable");
        SwitchDisplayRoomTable(false);
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

    /*#======================================================================#*/
    /*#    function : SwitchDisplayRoomTable  function                       #*/
    /*#    summary  : RoomTableの表示非表示を切り替える                      #*/
    /*#    argument : bool displayFlg                 -  表示非表示フラグ    #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SwitchDisplayRoomTable(bool displayFlg)
    {
        roomTable.SetActive(displayFlg);
    }

    /*#======================================================================#*/
    /*#    function : EnterTheRoom  function                                 #*/
    /*#    summary  : 部屋を設定して中に入る                                 #*/
    /*#    argument : string nickName                 - ニックネーム         #*/
    /*#               string roomName                 - ルーム名             #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SettingAndEnterTheRoom(string nickName, string roomName, byte maxPlayers)
    {
        //ニックネームを設定する
        SetMyNickName(nickName);

        //ルームオプション設定
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayers,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                {
                    "CustomProperties", "カスタムプロパティ"
                }
            },
            CustomRoomPropertiesForLobby = new string[]
            {
                "CustomProperties"
            }
        };

        //roomNameという名前のルームに参加する(ルームがなければ作成してから参加する)
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    /*#======================================================================#*/
    /*#    function : EnterTheRoom  function                                 #*/
    /*#    summary  : すでに作成済みの部屋に入る                             #*/
    /*#    argument : string nickName                 - ニックネーム         #*/
    /*#               string roomName                 - ルーム名             #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void EnterTheRoom(string nickName, string roomName)
    {
        //ニックネームを設定する
        SetMyNickName(nickName);

        //roomNameという名前のルームに参加する
        PhotonNetwork.JoinRoom(roomName);
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : SetMyNickName  function                                #*/
    /*#    summary  : ニックネームをつける                                   #*/
    /*#    argument : (I)string  nickName             -  ニックネーム        #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    private void SetMyNickName(string nickName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = nickName;
        }
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : joinLobby  function                                    #*/
    /*#    summary  : ロビーに入る                                           #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    private void joinLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                         Pun Callback List                          ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /* Photonに接続したとき */
    public override void OnConnected()
    {
        Debug.Log("ネットワークに接続しました。");
        SetMyNickName("guest player");
        Debug.Log("ようこそ、guest Playerさん。");
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
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        SceneManager.LoadScene("LobbyScene");
        
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
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("他のプレイヤーが入室してきました。");
    }

    /* 他のプレイヤーが退室したとき */
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("他のプレイヤーが退室しました。");
    }

    /* マスタークライアントが変わったとき */
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
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
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
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
