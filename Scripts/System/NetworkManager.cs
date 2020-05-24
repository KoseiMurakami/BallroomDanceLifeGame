using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    /*#======================================================================#*/
    /*#    function : GetRoomInfos  function                                 #*/
    /*#    summary  : room情報を取得する                                     #*/
    /*#    argument : (O) List<RoomInfo>  roomInfos   -  room情報            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void GetRoomInfos(ref List<RoomInfo> roomInfos)
    {
        OnRoomListUpdate(roomInfos);
    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                         Pun Callback List                          ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /* Photonに接続したとき */
    public override void OnConnected()
    {
        Debug.Log("ネットワークに接続しました。");
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
