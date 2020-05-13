using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkTest : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        //マッチング後、ランダムな位置に自分自身のネットワークオブジェクトを生成する
        var v = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("GamePlayer", v, Quaternion.identity);

        //遷移後のシーンでネットワークオブジェクトを生成する
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    public void ButtonPush()
    {
        //PhotonServerSettingに設定した内容を使ってマスターサーバに接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    //マスターサーバへの接続が成功したときに呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        //"testroom"という名前のルームに参加する(ルームがなければ作成してから参加する)
        PhotonNetwork.JoinOrCreateRoom("testroom", new RoomOptions(), TypedLobby.Default);
    }

    //マッチングが成功したときに呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        //遷移前のシーンでネットワークオブジェクトを生成しないようにする
        PhotonNetwork.IsMessageQueueRunning = false;
        //ルームに移動
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        //SceneManager.LoadScene("GameScene");
    }
}
