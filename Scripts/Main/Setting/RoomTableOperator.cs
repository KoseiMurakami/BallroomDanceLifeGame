using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WDT;

public class RoomTableOperator : MonoBehaviour
{
    [SerializeField]
    private SettingSceneManager settingSceneManager = default;

    private List<RoomInfo> roomInfos = new List<RoomInfo>();

    public WDataTable dataTable;
    public WRow wRow;
    public Text text;
    private List<IList<object>> m_datas = null;
    private List<WColumnDef> m_columnDefs = null;
    private int m_tempSelectIndex = -1;

    void Start()
    {
        m_datas = new List<IList<object>>();
        m_columnDefs = new List<WColumnDef>
        {
            new WColumnDef()
            {
                name = "部屋名",
                elementPrefabName = "TextElement",
                headPrefabName = "TextElement"
            },
            new WColumnDef()
            {
                name = "入室ボタン",
                elementPrefabName = "ButtonElement",
                headPrefabName = "TextElement"
            },
            new WColumnDef()
            {
                name = "ステータス",
                elementPrefabName = "TextElement",
                headPrefabName = "TextElement"
            },
        };
    }

    private void Update()
    {
    }

    /*#======================================================================#*/
    /*#    function : InitRoomTable  function                                #*/
    /*#    summary  : RoomTableのイニシャライズを行う                        #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void InitRoomTable()
    {
        List<object> temp_list;

        /* いったんm_dataの中身をクリア */
        m_datas.Clear();

        /* roomInfosを更新 */
        roomInfos = settingSceneManager.GetRoomInfos();

        if (roomInfos.Count == 0)
        {
            temp_list = new List<object>
            {
                "",
                "",
                ""
            };
            m_datas.Add(temp_list);
        }

        /* m_datasにデータを格納する */
        for (int i = 0; i < roomInfos.Count; i++)
        {
            temp_list = new List<object>
            {
                roomInfos[i].Name,
                "入室する",
                roomInfos[i].PlayerCount + "/ 20"
            };
            m_datas.Add(temp_list);
        }

        dataTable.msgHandle += HandleTableEvent;
        dataTable.InitDataTable(m_datas, m_columnDefs);
    }

    /*#======================================================================#*/
    /*#    function : GetRoomNameByCellIndex  function                       #*/
    /*#    summary  : rowIndexから部屋名を取得する                           #*/
    /*#    argument : int rowIndex                    -  横インデックス      #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public string GetRoomNameByCellIndex(int rowIndex)
    {
        return roomInfos[rowIndex].Name;
    }

    /*#======================================================================#*/
    /*#    function : HandleTableEvent  function                             #*/
    /*#    summary  : メッセージハンドルのイベント送信                       #*/
    /*#    argument : WEventType  messageType  :  メッセージ種別             #*/
    /*#               params object[]    args  :  オブジェクトパラメータ     #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void HandleTableEvent(WEventType messageType, params object[] args)
    {
        if (messageType == WEventType.INIT_ELEMENT)
        {
            int rowIndex = (int)args[0];
            int columnIndex = (int)args[1];
            WElement element = args[2] as WElement;
            if (element == null)
                return;
            Text tText = element.GetComponent<Text>();
            if (tText == null)
                return;
            tText.color = columnIndex % 2 == 0 ? Color.blue : Color.red;
        }
        else if (messageType == WEventType.SELECT_ROW)
        {
            int rowIndex = (int)args[0];
            if (text != null)
                text.text = "Select Row" + rowIndex;
            m_tempSelectIndex = rowIndex;
        }
    }
}
