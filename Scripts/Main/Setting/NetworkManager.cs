using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
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

    //マスターサーバへの接続が成功したときに呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        //roomNameという名前のルームに参加する(ルームがなければ作成してから参加する)
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
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
}
