/*#******************************************************************************#*/
/*#    SettingManager                                                            #*/
/*#                                                                              #*/
/*#    Summary    :    SettingSceneの管理                                        #*/
/*#******************************************************************************#*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private struct RoomsInfo
    {
        string  roomName;    //room名
        int membersCount;    //room内人数
    }

    private List<RoomsInfo> roomsInfoList = new List<RoomsInfo>();

    /*#======================================================================#*/
    /*#    function : BuildRoom  function                                    #*/
    /*#    summary  : ルームの作成を行う                                     #*/
    /*#    argument : string  roomName                -  ルーム名            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void BuildRoom(string roomName)
    {

    }
}
