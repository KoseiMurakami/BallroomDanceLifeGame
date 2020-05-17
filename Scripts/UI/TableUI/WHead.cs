using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace WDT
{
    public class WHead : WContainer
    {
        private HorizontalLayoutGroup m_hLayoutGroup;
        private RectTransform m_rectTransform;

        /*#======================================================================#*/
        /*#    function : InitContainer  function                                #*/
        /*#    summary  : コンテナ初期化処理                                     #*/
        /*#    argument : nothing                                                #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        protected override void InitContainer()
        {
            base.InitContainer();
            m_rectTransform = GetComponent<RectTransform>();
            m_hLayoutGroup = GetComponent<HorizontalLayoutGroup>();
            Assert.IsNotNull(m_rectTransform);
            Assert.IsNotNull(m_hLayoutGroup);
        }

        /*#======================================================================#*/
        /*#    function : GetObjectName  function                                #*/
        /*#    summary  : カラムインデックスからチューニングデータテーブルの     #*/
        /*#               ヘッダprefab名を取得する。                             #*/
        /*#    argument : int    columnIndex  -  カラムインデックス              #*/
        /*#    return   : string  objectName  -  ヘッダprefab名                  #*/
        /*#======================================================================#*/
        protected override string GetObjectName(int columnIndex)
        {
            if (bindDataTable == null || columnsDefs.Count <= 0)
                return "";

            if (columnIndex < 0 || columnIndex >= columnsDefs.Count)
                return "";

            string objectName = columnsDefs[columnIndex].headPrefabName;
            return string.IsNullOrEmpty(objectName) ? bindDataTable.defaultHeadPrefabName : objectName;
        }

        /*#======================================================================#*/
        /*#    function : UpdateHeadSize  function                               #*/
        /*#    summary  : チューニングテーブルヘッダのサイズをセットする。       #*/
        /*#    argument : nothing                                                #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        public void UpdateHeadSize()
        {
            if (bindDataTable == null || columnsDefs.Count <= 0)
                return;

            /* テーブルコンテナの要素サイズより少し大きめに調整 */
            m_rectTransform.sizeDelta = new Vector2(bindDataTable.tableWidth + 5, bindDataTable.itemHeight + 8);
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].SetSize(bindDataTable.GetWidthByColumnIndex(i), bindDataTable.itemHeight + 5);
            }
        }

        /*#======================================================================#*/
        /*#    function : SetColumnInfo  function                                #*/
        /*#    summary  : チューニングテーブルヘッダのカラム情報をセットする。   #*/
        /*#    argument : IList<WColumnDef> columnsDefsIn - カラム情報定義リスト #*/
        /*#               WDataTable        dataTable     - データテーブル       #*/
        /*#    return   : nothing                                                #*/
        /*#======================================================================#*/
        public void SetColumnInfo(IList<WColumnDef> columnsDefsIn, WDataTable dataTable)
        {
            bindDataTable = dataTable;
            if (!init)
                InitContainer();

            columnsDefs = columnsDefsIn;
            BuildChild();

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].SetInfo(columnsDefs[i].name, -1, i, bindDataTable);
            }

            UpdateHeadSize();
        }
    }
}

