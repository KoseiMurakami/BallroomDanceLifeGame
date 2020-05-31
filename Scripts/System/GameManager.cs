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
    string nickName;

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ゲームシーンをロードする。
    /// </summary>
    /// <param name="name">シーン名</param>
    public void LoadGameScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// ニックネームをセットする。
    /// </summary>
    /// <param name="name">ニックネーム名</param>
    public void SetMyNickName(string name)
    {
        this.nickName = name;
    }

    /// <summary>
    /// ニックネームを取得する。
    /// </summary>
    /// <returns>ニックネーム名</returns>
    public string GetMyNickName()
    {
        return this.nickName;
    }
}