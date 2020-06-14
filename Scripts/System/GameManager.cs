/*#**************************************************************************#*/
/*#    GameManager.cs                                                        #*/
/*#                                                                          #*/
/*#    Summary    :    ゲームの進行を管理                                    #*/
/*#                                                                          #*/
/*#**************************************************************************#*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public string NickName { set; get; }
    private Dictionary<string, int> playersScores = new Dictionary<string, int>();

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        playersScores.Clear();
    }

    /// <summary>
    /// ゲームシーンをロードする。
    /// </summary>
    /// <param name="name">シーン名</param>
    public void LoadGameScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetPlayersScores(string playerName, int points)
    {
        playersScores.Add(playerName, points);
    }

    public Dictionary<string, int> GetPlayersScores()
    {
        return this.playersScores;
    }
}