using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text roomName = default;

    [SerializeField]
    private Text membersCount = default;

    [SerializeField]
    private Text eventMessage = default;

    private GameObject myPlayerInLobby;
    private Vector3 instantiatePos = new Vector3();

    void Awake()
    {
        Debug.Log("LobbySceneManagerがインスタンス化されました。");

        roomName.text = PhotonNetwork.CurrentRoom.Name;
        membersCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() +
            " / " + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        eventMessage.text = PhotonNetwork.LocalPlayer.NickName + "さん。\n" +
            "ようこそ、 " + roomName.text + "へ";

        instantiatePos.x = Random.Range(-50.0f, 50.0f);
        instantiatePos.y = 5;
        instantiatePos.z = Random.Range(-50.0f, 50.0f);

        myPlayerInLobby = PhotonNetwork.Instantiate("Prefabs/NetworkObjects/GamePlayerForLobby",
                                                    instantiatePos,
                                                    Quaternion.identity);
        myPlayerInLobby.GetComponent<LobbyPlayer>();

        PhotonNetwork.IsMessageQueueRunning = true;

        StartCoroutine("keepText");
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (myPlayerInLobby != null)
            {
                PhotonNetwork.Destroy(myPlayerInLobby);
            }
            PhotonNetwork.IsMessageQueueRunning = false;

            GameManager.Instance.LoadGameScene("GameScene");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        eventMessage.text = newPlayer.NickName + "さんが入室しました。";
        StartCoroutine("keepText");
        membersCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() +
            " / " + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
    }

    private IEnumerator keepText()
    {
        yield return new WaitForSeconds(3);
        eventMessage.text = "";

        yield break;
    }

    public GameObject GetMyPlayerObject()
    {
        return myPlayerInLobby;
    }
}
