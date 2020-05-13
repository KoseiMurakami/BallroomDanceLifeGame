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

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : LoadGameScene  function                                #*/
    /*#    summary  : ゲームシーンをロードする                               #*/
    /*#    argument : (O)string  name                 -  シーン名            #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    public void LoadGameScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}