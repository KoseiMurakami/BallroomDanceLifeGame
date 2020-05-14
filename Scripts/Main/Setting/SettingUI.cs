using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject buildRoomPanelObject = default;

    [SerializeField]
    private SettingManager settingManager = default;

    [SerializeField]
    private InputField inputField = default;

    /*#======================================================================#*/
    /*#    function : PushBuildRoomButton  function                          #*/
    /*#    summary  : BuildRoomButtonを押したときの処理                      #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushBuildRoomButton()
    {
        buildRoomPanelObject.SetActive(true);
    }

    /*#======================================================================#*/
    /*#    function : PushBackButton  function                               #*/
    /*#    summary  : BackButtonを押したときの処理                           #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushBackButton()
    {
        buildRoomPanelObject.SetActive(false);
    }

    /*#======================================================================#*/
    /*#    function : PushEnterButton  function                              #*/
    /*#    summary  : EnterButtonを押したときの処理                          #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushEnterButton()
    {
        settingManager.SetRoomName(inputField.text);
        settingManager.EnterTheRoom();
    }
}
