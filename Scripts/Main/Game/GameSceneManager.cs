using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LifeGame;

public class GameSceneManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    [SerializeField]
    private GameObject startPoint = default;

    [SerializeField]
    private Text diceEyesText = default;

    [SerializeField]
    private GameObject eventTextPanel = default;

    [SerializeField]
    private GameObject shopTextPanel = default;

    [SerializeField]
    private GameObject compeTextPanel = default;

    [SerializeField]
    private GameObject accountingPanel = default;

    [SerializeField]
    private MainCamera mainCamera = default;

    [SerializeField]
    private GameObject canvas = default;

    [SerializeField]
    private GameObject billboard = default;

    private PunTurnManager turnManager;
    private GameObject playerPrefab;
    private GameObject[] playerStatusPanelPrefab;
    private readonly List<GameObject> playerPanelList = new List<GameObject>();
    private GameObject playerObject;
    private readonly List<GameObject> playerObjectList = new List<GameObject>();
    private GamePlayer gamePlayer;
    private readonly List<GamePlayer> gamePlayerList = new List<GamePlayer>();
    private int activePlayerIndex;
    private Vector3 instantiatePos;
    private readonly int brotherPrice = 30;
    private readonly int partnerPrice = 30;
    private readonly int buybyMoney = 40;

    private void Awake()
    {
        turnManager = GetComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        turnManager.TurnDuration = 30f;

        //turnManagerを起動するのに必須
        PhotonNetwork.IsMessageQueueRunning = true;

        playerPrefab = Resources.Load<GameObject>("Prefabs/NetworkObjects/GamePlayer");
        playerStatusPanelPrefab = Resources.LoadAll<GameObject>("Prefabs/UI/PlayerStatus");

        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            //プレイヤー生成の起点
            instantiatePos = new Vector3(startPoint.transform.position.x + 2 * Mathf.Sin(i * Mathf.PI / 2),
                                         startPoint.transform.position.y + 1,
                                         startPoint.transform.position.z + 2 * Mathf.Cos(i * Mathf.PI / 2));

            //プレイヤー生成＆情報キャッシュ
            playerObject = Instantiate(playerPrefab, instantiatePos, Quaternion.Euler(0,0,0));
            playerObjectList.Add(playerObject);
            gamePlayer = playerObject.GetComponent<GamePlayer>();
            gamePlayerList.Add(gamePlayer);

            //ステータスパネル人数分生成(親要素をcanvasに設定しないと表示されない)
            GameObject panel = Instantiate(playerStatusPanelPrefab[i]);
            panel.transform.SetParent(canvas.transform, false);
            playerPanelList.Add(panel);

            //プレイヤー情報を設定
            gamePlayer.PlayerId = i;
            gamePlayer.PlayerName = PhotonNetwork.CurrentRoom.Players[i + 1].NickName;

            //プレイヤーパネルの設定
            Text playerName = panel.transform.Find("PlayerName").GetComponent<Text>();
            Text moneyPoints = panel.transform.Find("MoneyPoints/MoneyPointsText").GetComponent<Text>();
            Text dancePoints = panel.transform.Find("DancePoints/DancePointsText").GetComponent<Text>();
            Text popularityPoints = panel.transform.Find("PopularityPoints/PopularityPointsText").GetComponent<Text>();
            Text lovePoints = panel.transform.Find("LovePoints/LovePointsText").GetComponent<Text>();
            playerName.text = gamePlayer.PlayerName;
            moneyPoints.text = "0";
            dancePoints.text = "0";
            popularityPoints.text = "0";
            lovePoints.text = "0";
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.CompareTag("billboard"))
                {
                    billboard.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// ゲーム開始リクエストする
    /// </summary>
    public void RequireActStart()
    {
        //マスタークライアントがターンを開始する
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("マスタークライアントがターンを開始しました。");
            turnManager.BeginTurn();
        }
    }

    /// <summary>
    /// さいころをふる
    /// </summary>
    public void RollTheDice()
    {
        if (isThisMyTurn())
        {
            int eyes;
            eyes = Random.Range(1, 11) +
                gamePlayerList[activePlayerIndex].GetLanks(LankKindDef.BrotherLank);

            ////プレイヤーカスタムプロパティの更新
            var hashtable = new ExitGames.Client.Photon.Hashtable()
            {
                ["DiceEyes"] = eyes
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    /// <summary>
    /// マスの残り数を表示する
    /// </summary>
    /// <param name="eyes"></param>
    public void DisplayRestDiceEyes(int eyes)
    {
        diceEyesText.text = eyes.ToString();
    }

    /// <summary>
    /// EventTextPanelを表示する
    /// </summary>
    /// <param name="text">表示テキスト</param>
    public void DisplayEventTextPanel(string text)
    {
        eventTextPanel.SetActive(true);
        Text displayText = eventTextPanel.transform.Find("EventText").GetComponent<Text>();

        displayText.text = text;
    }

    /// <summary>
    /// ShopTextPanelを表示する
    /// </summary>
    /// <param name="text">表示テキスト</param>
    public void DisplayShopTextPanel(SquareKindDef squareKind, string text)
    {
        //引数付きイベントリスナー設定
        GameObject yesButton = shopTextPanel.transform.Find("YesButton").gameObject;
        GameObject noButton = shopTextPanel.transform.Find("NoButton").gameObject;

        shopTextPanel.SetActive(true);
        Text displayText = shopTextPanel.transform.Find("ShopText").GetComponent<Text>();

        if (isThisMyTurn())
        {
            displayText.text = text + '\n' + "買収しますか？";
            yesButton.SetActive(true);
            noButton.SetActive(true);

            yesButton.GetComponent<Button>().onClick.AddListener(() => {PushYesButton(squareKind);});
            noButton.GetComponent<Button>().onClick.AddListener(() => {PushNoButton();});
        }
        else
        {
            displayText.text = text + '\n'
                + gamePlayerList[activePlayerIndex].PlayerName + "さんが手続き中......";
            yesButton.SetActive(false);
            noButton.SetActive(false);
        }
    }

    /// <summary>
    /// CompeTextPanelを表示する
    /// </summary>
    /// <param name="text"></param>
    /// <param name="dancePoint"></param>
    public void DisplayCompeTextPanel(string text, int dancePoint)
    {
        GameObject compeButton = compeTextPanel.transform.Find("CompeButton").gameObject;
        GameObject textButton = compeTextPanel.transform.Find("TextButton").gameObject;

        compeTextPanel.SetActive(true);
        textButton.SetActive(false);
        Text displayText = compeTextPanel.transform.Find("CompeText").GetComponent<Text>();
        Text resultText = compeTextPanel.transform.Find("ResultText").GetComponent<Text>();
        resultText.text = "";

        if (isThisMyTurn())
        {
            displayText.text = text;
            compeButton.SetActive(true);

            compeButton.GetComponent<Button>().onClick.AddListener(() => { PushCompeButton(dancePoint); });
        }
        else
        {
            displayText.text = text + '\n'
                + gamePlayerList[activePlayerIndex].PlayerName + "さんが試合中......";
            compeButton.SetActive(false);
        }
    }

    /// <summary>
    /// アクティブプレイヤーインデックスをセットする
    /// </summary>
    private void SetActivePlayerIndex()
    {
        //1ターン目から始まるので"-1"しておく
        activePlayerIndex = (turnManager.Turn - 1) % PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    /// <summary>
    /// このターンが自分のターンか確認する
    /// </summary>
    /// <returns></returns>
    private bool isThisMyTurn()
    {
        string nowPlayerId = PhotonNetwork.CurrentRoom.Players[activePlayerIndex + 1].UserId;

        if (nowPlayerId == PhotonNetwork.LocalPlayer.UserId)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// プレイヤーパネルのポイント情報を更新する
    /// </summary>
    /// <param name="pointKind"></param>
    /// <param name="points"></param>
    public void UpdatePlayerPanel(PointKindDef pointKind, int points)
    {
        Debug.Log("UpdatePlayerPanel : " + pointKind + " , " + points);
        //アクティブプレイヤーにポイント加算
        gamePlayerList[activePlayerIndex].SetAddPoints(pointKind, points);
        //プレイヤーパネルの子要素を検索してポイント追加
        Text pointText = playerPanelList[activePlayerIndex].transform.
            Find(pointKind + "/" + pointKind + "Text").GetComponent<Text>();
        pointText.text = gamePlayerList[activePlayerIndex].GetPoints(pointKind).ToString();
    }

    /// <summary>
    /// プレイヤーパネルのランク情報を更新する
    /// </summary>
    /// <param name="lankKind"></param>
    /// <param name="lanks"></param>
    public void UpdatePlayerPanel(LankKindDef lankKind, int lanks)
    {
        Debug.Log("UpdatePlayerPanel : " + lankKind + " , " + lanks);
        //アクティブプレイヤーにランク加算
        gamePlayerList[activePlayerIndex].SetAddLanks(lankKind, lanks);
        //プレイヤーパネルの子要素を検索してポイント追加
        Text pointText = playerPanelList[activePlayerIndex].transform.
            Find(lankKind + "/" + lankKind + "Text").GetComponent<Text>();
        pointText.text = gamePlayerList[activePlayerIndex].GetLanks(lankKind).ToString();
    }

    /// <summary>
    /// AccountPanel用のドロップダウンリストをセットする
    /// </summary>
    /// <param name="dropdown"></param>
    /// <param name="maxListCount"></param>
    private void setDropdownListForAccountPanel(Dropdown dropdown, int maxListCount)
    {
        dropdown.ClearOptions();
        List<string> list = new List<string>();
        for (int i = 0; i <= maxListCount; i++)
        {
            list.Add(i.ToString() + "人");
        }
        dropdown.AddOptions(list);
    }

    /// <summary>
    /// ドロップダウンのMAXリスト数を算出する
    /// </summary>
    /// <param name="price"></param>
    /// <param name="currency"></param>
    /// <returns></returns>
    private int calcMaxListCount(int price, int currency)
    {
        int maxListCount = 0;
        if (currency > 0)
        {
            maxListCount = currency / price;
        }

        return maxListCount;
    }

    /// <summary>
    /// TextButtonを押したときの処理
    /// </summary>
    public void PushTextButton()
    {
        eventTextPanel.SetActive(false);
        turnManager.SendMove(0, true);
    }

    /// <summary>
    /// YesButtonを押したときの処理
    /// </summary>
    /// <param name="lankKind"></param>
    public void PushYesButton(SquareKindDef squareKind)
    {
        Text textA;
        Text textB;
        Dropdown dropdownA;
        Dropdown dropdownB;
        int maxListCount = 0;

        //LankKindの料金表を表示する
        shopTextPanel.SetActive(false);
        accountingPanel.SetActive(true);

        GameObject productKindTextA =
                    accountingPanel.transform.Find("ProductKindTextA").gameObject;
        GameObject productKindTextB =
                   accountingPanel.transform.Find("ProductKindTextB").gameObject;
        GameObject productABuyCount =
            accountingPanel.transform.Find("ProductKindTextA/ProductABuyCount").gameObject;
        GameObject productBBuyCount =
            accountingPanel.transform.Find("ProductKindTextB/ProductBBuyCount").gameObject;
        GameObject decideButton =
            accountingPanel.transform.Find("DecideButton").gameObject;

        switch (squareKind)
        {
            case SquareKindDef.brotherShop:
                productKindTextA.SetActive(true);
                productABuyCount.SetActive(true);
                productKindTextB.SetActive(false);
                productBBuyCount.SetActive(false);

                dropdownA = productABuyCount.GetComponent<Dropdown>();
                dropdownA.onValueChanged.RemoveAllListeners();
                maxListCount = calcMaxListCount(brotherPrice, gamePlayerList[activePlayerIndex].GetPoints(PointKindDef.PopularityPoints));
                setDropdownListForAccountPanel(dropdownA, maxListCount);

                textA = productKindTextA.GetComponent<Text>();
                textA.text = "舎弟(1人につき人望P" + brotherPrice +"消費)";
                break;
            case SquareKindDef.partnerShop:
                productKindTextA.SetActive(true);
                productABuyCount.SetActive(true);
                productKindTextB.SetActive(false);
                productBBuyCount.SetActive(false);

                dropdownA = productABuyCount.GetComponent<Dropdown>();
                dropdownA.onValueChanged.RemoveAllListeners();
                maxListCount = calcMaxListCount(partnerPrice, gamePlayerList[activePlayerIndex].GetPoints(PointKindDef.LovePoints));
                setDropdownListForAccountPanel(dropdownA, maxListCount);

                textA = productKindTextA.GetComponent<Text>();
                textA.text = "愛人(1人につき恋愛P" + partnerPrice + "消費)";
                break;
            case SquareKindDef.dualShop:
                productKindTextA.SetActive(true);
                productABuyCount.SetActive(true);
                productKindTextB.SetActive(true);
                productBBuyCount.SetActive(true);

                dropdownA = productABuyCount.GetComponent<Dropdown>();
                dropdownB = productBBuyCount.GetComponent<Dropdown>();
                dropdownA.onValueChanged.RemoveAllListeners();
                dropdownB.onValueChanged.RemoveAllListeners();
                maxListCount = calcMaxListCount(buybyMoney, gamePlayerList[activePlayerIndex].GetPoints(PointKindDef.MoneyPoints));
                setDropdownListForAccountPanel(dropdownA, maxListCount);
                setDropdownListForAccountPanel(dropdownB, maxListCount);
                //dropdownA.onValueChanged.AddListener(delegate
                //{
                //    DropdownAValueChanged();
                //});
                //dropdownB.onValueChanged.AddListener(delegate
                //{
                //    DropdownBValueChanged();
                //});
                textA = productKindTextA.GetComponent<Text>();
                textB = productKindTextB.GetComponent<Text>();
                textA.text = "舎弟(1人につき資金" + buybyMoney + "消費)";
                textB.text = "愛人(1人につき資金" + buybyMoney + "消費)";
                break;
        }

        decideButton.GetComponent<Button>().onClick.RemoveAllListeners();
        decideButton.GetComponent<Button>().onClick.AddListener(() => { PushDecideButton(squareKind); });
    }

    /// <summary>
    /// DecideButtonを押したときの処理
    /// </summary>
    /// <param name="lankKind"></param>
    public void PushDecideButton(SquareKindDef squareKind)
    {
        if (!isThisMyTurn())
        {
            return;
        }

        Dropdown productABuyCount =
            accountingPanel.transform.Find("ProductKindTextA/ProductABuyCount").gameObject.GetComponent<Dropdown>();
        Dropdown productBBuyCount =
            accountingPanel.transform.Find("ProductKindTextB/ProductBBuyCount").gameObject.GetComponent<Dropdown>();

        switch (squareKind)
        {
            case SquareKindDef.brotherShop:
                var hashtable = new ExitGames.Client.Photon.Hashtable()
                {
                    ["Shop"] = true,
                    ["BrotherLank"] = productABuyCount.value,
                    ["PartnerLank"] = 0,
                    ["PopularityPoints"] = productABuyCount.value * brotherPrice,
                    ["LovePoints"] = 0,
                    ["MoneyPoints"] = 0,
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                break;
            case SquareKindDef.partnerShop:
                hashtable = new ExitGames.Client.Photon.Hashtable()
                {
                    ["Shop"] = true,
                    ["BrotherLank"] = 0,
                    ["PartnerLank"] = productABuyCount.value,
                    ["PopularityPoints"] = 0,
                    ["LovePoints"] = productABuyCount.value * partnerPrice,
                    ["MoneyPoints"] = 0
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                break;
            case SquareKindDef.dualShop:

                if (gamePlayerList[activePlayerIndex].GetPoints(PointKindDef.MoneyPoints) -
                    (productABuyCount.value + productBBuyCount.value) * buybyMoney < 0)
                {
                    return;
                }

                hashtable = new ExitGames.Client.Photon.Hashtable()
                {
                    ["Shop"] = true,
                    ["BrotherLank"] = productABuyCount.value,
                    ["PartnerLank"] = productBBuyCount.value,
                    ["PopularityPoints"] = 0,
                    ["LovePoints"] = 0,
                    ["MoneyPoints"] = (productABuyCount.value + productBBuyCount.value) * buybyMoney
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                break;
        }

        accountingPanel.SetActive(false);
    }

    /// <summary>
    /// NoButtonを押したときの処理
    /// </summary>
    public void PushNoButton()
    {
        var hashtable = new ExitGames.Client.Photon.Hashtable()
        {
            ["Shop"] = false
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    /// <summary>
    /// CompeButtonを押したときの処理
    /// </summary>
    /// <param name="dancePoint"></param>
    public void PushCompeButton(int dancePoint)
    {
        CompeResult result = new CompeResult();
        float probability = 0;
        GameObject compeButton = compeTextPanel.transform.Find("CompeButton").gameObject;
        GameObject textButton = compeTextPanel.transform.Find("TextButton").gameObject;
        Text resultText = compeTextPanel.transform.Find("ResultText").GetComponent<Text>();

        compeButton.SetActive(false);
        textButton.SetActive(true);

        probability = Random.Range(1,11);
        if (dancePoint < 100)
        {
            if (probability >= 9)
            {
                result = CompeResult.SemiFinalist;
            }
            else
            {
                result = CompeResult.LostQualifying;
            }
        }
        else if (dancePoint < 200)
        {
            if (probability >= 10)
            {
                result = CompeResult.Champion;
            }
            else if (probability >= 5)
            {
                result = CompeResult.Finalist;
            }
            else if (probability >= 2)
            {
                result = CompeResult.SemiFinalist;
            }
            else
            {
                result = CompeResult.LostQualifying;
            }
        }
        else
        {
            if (probability >= 8)
            {
                result = CompeResult.Champion;
            }
            else if (probability >= 4)
            {
                result = CompeResult.Finalist;
            }
            else if (probability >= 2)
            {
                result = CompeResult.SemiFinalist;
            }
            else
            {
                result = CompeResult.LostQualifying;
            }
        }

        resultText.text = result.ToString();
        textButton.GetComponent<Button>().onClick.RemoveAllListeners();
        textButton.GetComponent<Button>().onClick.AddListener(() => {PushTextButtonInCompe(result);});
    }

    /// <summary>
    /// CompeTextPanelのTextButtonを押したときの処理
    /// </summary>
    /// <param name="result"></param>
    public void PushTextButtonInCompe(CompeResult result)
    {
        if (!isThisMyTurn())
        {
            return;
        }
        Debug.Log("called PushTextButtonInCompe.");
        var hashtable = new ExitGames.Client.Photon.Hashtable()
        {
            ["CompeResult"] = result
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    /// <summary>
    /// すべてのプレイヤーがゴールしているかどうか調べる
    /// </summary>
    /// <returns>bool 全プレイヤーがゴールしているか</returns>
    private bool hasAllPlayerFinished()
    {
        foreach(GamePlayer gamePlayer in gamePlayerList)
        {
            if (!gamePlayer.FinishedFlg)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// DropdownAが変更されたときの処理
    /// </summary>
    public void DropdownAValueChanged()
    {
        Dropdown productBBuyCount =
            accountingPanel.transform.Find("ProductKindTextB/ProductBBuyCount").gameObject.GetComponent<Dropdown>();

        int tempCount = productBBuyCount.value;
        int maxListCount = calcMaxListCount(buybyMoney, gamePlayerList[activePlayerIndex].GetPoints(PointKindDef.MoneyPoints) - partnerPrice * productBBuyCount.value);
        
        setDropdownListForAccountPanel(productBBuyCount, maxListCount);

        if (tempCount <= maxListCount)
        {
            productBBuyCount.value = tempCount;
        }
        else
        {
            productBBuyCount.value = maxListCount;
        }
    }

    /// <summary>
    /// DropdownBが変更されたときの処理
    /// </summary>
    public void DropdownBValueChanged()
    {
        Dropdown productABuyCount =
            accountingPanel.transform.Find("ProductKindTextA/ProductABuyCount").gameObject.GetComponent<Dropdown>();

        int tempCount = productABuyCount.value;
        int maxListCount = calcMaxListCount(buybyMoney, gamePlayerList[activePlayerIndex].GetPoints(PointKindDef.MoneyPoints) - brotherPrice * productABuyCount.value);

        setDropdownListForAccountPanel(productABuyCount, maxListCount);

        if (tempCount <= maxListCount)
        {
            productABuyCount.value = tempCount;
        }
        else
        {
            productABuyCount.value = maxListCount;
        }
    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                     TurnManager Callback List                      ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    //ターンが開始したとき
    public void OnTurnBegins(int turn)
    {
        SetActivePlayerIndex();
        mainCamera.SetTargetObj(playerObjectList[activePlayerIndex]);

        //すでにアクティブプレイヤーがゴールしていたときの処理
        if (gamePlayerList[activePlayerIndex].FinishedFlg)
        {
            gamePlayerList[activePlayerIndex].GetFinishedBonus();
            return;
        }

        if (isThisMyTurn())
        {
            //自分のターン処理
            gamePlayerList[activePlayerIndex].SwitchDiceButtonDisplay(true);
        }
        else
        {

        }
    }

    //ターンのムーブが全プレイヤー完了したとき
    public void OnTurnCompleted(int turn)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.BeginTurn();
        }
    }

    //プレイヤーのムーブを開始したとき
    public void OnPlayerMove(Player player, int turn, object move)
    {

    }

    //プレイヤーのムーブが終了したとき
    public void OnPlayerFinished(Player player, int turn, object move)
    {
        //ターン終了時、すべてのプレイヤーがゴールしていれば
        if (turnManager.Turn % PhotonNetwork.CurrentRoom.MaxPlayers == 0
            && hasAllPlayerFinished())
        {
            Debug.Log("全プレイヤーがゴールしました。");

            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                int totalPoints;

                Dictionary<PointKindDef, int> points = gamePlayerList[i].GetPoints();

                totalPoints = points[PointKindDef.MoneyPoints] +
                               points[PointKindDef.DancePoints] +
                               points[PointKindDef.PopularityPoints] +
                               points[PointKindDef.LovePoints];

                GameManager.Instance.SetPlayersScores(gamePlayerList[i].PlayerName, totalPoints);
            }

            GameManager.Instance.LoadGameScene("ResultScene");
        }
    }

    //タイマーがタイムアウトしたときに処理したい内容を書く
    public void OnTurnTimeEnds(int turn)
    {

    }

    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    /*★                         Pun Callback List                          ★*/
    /*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★*/
    //カスタムプロパティの値が変更されたとき
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("called OnPlayerPropertiesUpdate");
        //DiceEyesが設定されている場合
        if (changedProps.TryGetValue("DiceEyes", out object diceEyesObject))
        {
            int eyes = (int)diceEyesObject;
            diceEyesText.text = eyes.ToString();
            gamePlayerList[activePlayerIndex].setTheDice(eyes);
        }

        //Shopで購入があった場合
        if (changedProps.TryGetValue("Shop", out object shopObject))
        {
            shopTextPanel.SetActive(false);

            if ((bool)shopObject)
            {
                int buyBrotherCount = 0;
                int buyPartnerCount = 0;
                int usePopularityPoints = 0;
                int useLovePoints = 0;
                int useMoneyPoints = 0;

                if (changedProps.TryGetValue("BrotherLank", out object buyBrotherObject))
                {
                    buyBrotherCount = (int)buyBrotherObject;
                }

                if (changedProps.TryGetValue("PartnerLank", out object buyPartnerObject))
                {
                    buyPartnerCount = (int)buyPartnerObject;
                }

                if (changedProps.TryGetValue("PopularityPoints", out object PopularityPointsObject))
                {
                    usePopularityPoints = (int)PopularityPointsObject;
                }

                if (changedProps.TryGetValue("LovePoints", out object LovePointsObject))
                {
                    useLovePoints = (int)LovePointsObject;
                }

                if (changedProps.TryGetValue("MoneyPoints", out object MoneyPointsObject))
                {
                    useMoneyPoints = (int)MoneyPointsObject;
                }

                if (useMoneyPoints > 0)
                {
                    Debug.Log("dualshopマス : 舎弟" + buyBrotherCount + "人、愛人" + buyPartnerCount + "人を購入しました。");
                    Debug.Log("called by dualshop.");
                    UpdatePlayerPanel(PointKindDef.MoneyPoints, -useMoneyPoints);
                    UpdatePlayerPanel(LankKindDef.BrotherLank, buyBrotherCount);
                    UpdatePlayerPanel(LankKindDef.PartnerLank, buyPartnerCount);
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんが" + '\n' +
                        "舎弟を" + buyBrotherCount.ToString() + "人" + '\n' +
                        "愛人を" + buyPartnerCount.ToString() + "人" + '\n' +
                        "買収しました。");
                }
                else if (usePopularityPoints > 0)
                {
                    Debug.Log("brotherShopマス : 舎弟" + buyBrotherCount + "人を購入しました。");
                    Debug.Log("called by brothershop.");
                    UpdatePlayerPanel(PointKindDef.PopularityPoints, -usePopularityPoints);
                    UpdatePlayerPanel(LankKindDef.BrotherLank, buyBrotherCount);
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんが" + '\n' +
                        "舎弟を" + buyBrotherCount.ToString() + "人買収しました。");
                }
                else if (useLovePoints > 0)
                {
                    Debug.Log("partnerShopマス : 愛人" + buyPartnerCount + "人を購入しました。");
                    Debug.Log("called by partnershop.");
                    UpdatePlayerPanel(PointKindDef.LovePoints, -useLovePoints);
                    UpdatePlayerPanel(LankKindDef.PartnerLank, buyPartnerCount);
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんが" + '\n' +
                    "愛人を" + buyPartnerCount.ToString() + "人買収しました。");
                }
                else
                {
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName +
                        "さんは買収しませんでした。");
                }
            }
            else
            {
                DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName +
                            "さんは買収しませんでした。");
            }
        }

        //試合があった場合
        if (changedProps.TryGetValue("CompeResult", out object resultObject))
        {
            compeTextPanel.SetActive(false);
            CompeResult result = (CompeResult)resultObject;

            switch (result)
            {
                case CompeResult.LostQualifying:
                    Debug.Log("called by compeResult : LostQualifying.");
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんは予選落ちでした。");
                    break;
                case CompeResult.SemiFinalist:
                    Debug.Log("called by compeResult : SemiFinalist.");
                    UpdatePlayerPanel(PointKindDef.MoneyPoints, 5);
                    UpdatePlayerPanel(PointKindDef.DancePoints, 5);
                    UpdatePlayerPanel(PointKindDef.PopularityPoints, 5);
                    UpdatePlayerPanel(PointKindDef.LovePoints, 5);
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんはセミファイナリストになりました！" + '\n' +
                        "全ポイント +5P");
                    break;
                case CompeResult.Finalist:
                    Debug.Log("called by compeResult : Finalist.");
                    UpdatePlayerPanel(PointKindDef.MoneyPoints, 10);
                    UpdatePlayerPanel(PointKindDef.DancePoints, 10);
                    UpdatePlayerPanel(PointKindDef.PopularityPoints, 10);
                    UpdatePlayerPanel(PointKindDef.LovePoints, 10);
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんはファイナリストになりました！！！" + '\n' +
                        "全ポイント +10P");
                    break;
                case CompeResult.Champion:
                    Debug.Log("called by compeResult : Champoion.");
                    UpdatePlayerPanel(PointKindDef.MoneyPoints, 15);
                    UpdatePlayerPanel(PointKindDef.DancePoints, 15);
                    UpdatePlayerPanel(PointKindDef.PopularityPoints, 15);
                    UpdatePlayerPanel(PointKindDef.LovePoints, 15);
                    DisplayEventTextPanel(gamePlayerList[activePlayerIndex].PlayerName + "さんは見事優勝を飾りました！！！！" + '\n' +
                        "全ポイント +15P");
                    break;
            }
        }
    }
}
