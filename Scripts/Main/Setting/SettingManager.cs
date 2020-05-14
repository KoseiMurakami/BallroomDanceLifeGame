/*#******************************************************************************#*/
/*#    SettingManager                                                            #*/
/*#                                                                              #*/
/*#    Summary    :    SettingSceneの管理                                        #*/
/*#******************************************************************************#*/
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviourPunCallbacks
{
    private List<RoomInfo> roomInfos = new List<RoomInfo>();
    private string roomName;

    /*#======================================================================#*/
    /*#    function : EnterTheRoom  function                                 #*/
    /*#    summary  : 部屋に入る                                             #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void EnterTheRoom()
    {
        //PhotonServerSettingに設定した内容を使ってマスターサーバに接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    

    //マッチングが成功したときに呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        //遷移前のシーンでネットワークオブジェクトを生成しないようにする
        PhotonNetwork.IsMessageQueueRunning = false;
        //ルームに移動
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        SceneManager.LoadScene("GameScene");
    }

    /*#======================================================================#*/
    /*#    function : SetRoomName  function                                  #*/
    /*#    summary  : RoomNameをセットする                                   #*/
    /*#    argument : (I)string  name                 -  ルーム名            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SetRoomName(string name)
    {
        roomName = name;
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

    /* マスターサーバへの接続したとき */
    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターサーバに接続しました。");

        //roomNameという名前のルームに参加する(ルームがなければ作成してから参加する)
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
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

}
