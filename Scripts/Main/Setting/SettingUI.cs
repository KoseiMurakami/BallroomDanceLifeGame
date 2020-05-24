using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField]
    private SettingSceneManager settingSceneManager = default;

    [SerializeField]
    private RoomTableOperator roomTableOperator = default;

    [SerializeField]
    private InputField inputField = default;

    [SerializeField]
    private Text membersCount = default;

    /*#======================================================================#*/
    /*#    function : PushEnterButton  function                              #*/
    /*#    summary  : EnterButtonを押したときの処理                          #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushEnterButton()
    {
        settingSceneManager.SettingAndEnterTheRoom("ゲストプレイヤー",
            inputField.text, Convert.ToByte(membersCount.text));
    }

    /*#======================================================================#*/
    /*#    function : PushEnterButtonInTable  function                       #*/
    /*#    summary  : Table上のEnterButtonを押したときの処理                 #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushEnterButtonInTable(int rowIndex, int columnIndex)
    {
        Debug.Log(rowIndex + "," + columnIndex + "is selected");

        settingSceneManager.EnterTheRoom("ゲストプレイヤー",
            roomTableOperator.GetRoomNameByCellIndex(rowIndex));
    }

    /*#======================================================================#*/
    /*#    function : PushSeekRoomButton  function                           #*/
    /*#    summary  : SeekRoomButtonを押したときの処理                       #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushSeekRoomButton()
    {
        settingSceneManager.SwitchDisplayRoomTable(true);
        roomTableOperator.InitRoomTable();
    }
}
