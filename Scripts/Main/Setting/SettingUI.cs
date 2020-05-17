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

    /*#======================================================================#*/
    /*#    function : PushEnterButton  function                              #*/
    /*#    summary  : EnterButtonを押したときの処理                          #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void PushEnterButton()
    {
        settingSceneManager.EnterTheRoom(inputField.text);
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

        settingSceneManager.EnterTheRoom(roomTableOperator.GetRoomNameByCellIndex(rowIndex));
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
