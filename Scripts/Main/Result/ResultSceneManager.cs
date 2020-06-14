using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject canvas = default;

    private Dictionary<string, int> playersScores = new Dictionary<string, int>();
    private GameObject resultTextPrefab;

    void Start()
    {
        playersScores = GameManager.Instance.GetPlayersScores();

        var sortedPlayersScores = playersScores.OrderByDescending((player) => player.Value);

        resultTextPrefab = Resources.Load<GameObject>("Prefabs/UI/ResultText");

        int i = 0;
        foreach (var player in sortedPlayersScores)
        {
            GameObject obj = Instantiate(resultTextPrefab);
            obj.transform.SetParent(canvas.transform, false);
            obj.transform.position += new Vector3(0, - 65 * i, 0);
            obj.GetComponent<Text>().text = (i + 1).ToString() + "位  " + player.Key + " さん    " + player.Value + "点";
            i++;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (PhotonNetwork.IsConnected)
            {
                //Photonルームから出る
                PhotonNetwork.LeaveRoom();
                //Photonロビーから出る
                PhotonNetwork.LeaveLobby();
                //Photonネットワークの接続を切る
                PhotonNetwork.Disconnect();
            }

            //ホームへ
            GameManager.Instance.LoadGameScene("OpenningScene");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "さんが退室しました。");
    }
}
