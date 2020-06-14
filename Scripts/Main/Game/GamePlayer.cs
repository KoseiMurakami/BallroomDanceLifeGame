using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using LifeGame;
using System.Collections.Generic;

public class GamePlayer : MonoBehaviour
{
    public int PlayerId { set; get; }
    public string PlayerName { set; get; }

    private readonly Dictionary<LankKindDef, int> lanks = new Dictionary<LankKindDef, int>();
    private readonly Dictionary<PointKindDef, int> points = new Dictionary<PointKindDef, int>();

    public bool FinishedFlg { set; get; }

    private float speed;

    private GameSceneManager gameSceneManager;
    static private Button diceButton;
    static private Text eyesText;
    private Animator animator;
    private Rigidbody rb;
    private Collider squareCollider = new Collider();

    private bool arrivedPosFlg = true;
    private bool walkingFlg = false;
    private int restEyes = 0;
    private Vector3 nextPosition;
    private Vector3 warpPosition;
    private readonly int goalBonus = 50;
    private readonly int donation = 3;

    void Start()
    {
        gameSceneManager = 
            GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        diceButton = GameObject.Find("DiceButton").GetComponent<Button>();
        eyesText = GameObject.Find("DiceEyesText").GetComponent<Text>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        FinishedFlg = false;
        speed = 0;
        eyesText.text = "0";
        lanks.Add(LankKindDef.BrotherLank, 0);
        lanks.Add(LankKindDef.PartnerLank, 0);
        points.Add(PointKindDef.MoneyPoints, 0);
        points.Add(PointKindDef.DancePoints, 0);
        points.Add(PointKindDef.PopularityPoints, 0);
        points.Add(PointKindDef.LovePoints, 0);

        //次のマスの方向を向く
        this.transform.LookAt(nextPosition);

        SwitchDiceButtonDisplay(false);
    }

    void Update()
    {
        animator.SetFloat("speed", speed);

        if (restEyes > 0 && !walkingFlg && !FinishedFlg)
        {
            StartCoroutine("walkToNextSquare");
        }
    }

    /// <summary>
    /// ダイスの目をセットする
    /// </summary>
    /// <param name="eyes"></param>
    public void setTheDice(int eyes)
    {
        restEyes = eyes;
        SwitchDiceButtonDisplay(false);
    }

    /// <summary>
    /// ランク情報をセットする
    /// </summary>
    /// <param name="lankKind"></param>
    /// <param name="value"></param>
    public void SetAddLanks(LankKindDef lankKind, int value)
    {
        lanks[lankKind] += value;
        if (lanks[lankKind] < 0)
        {
            lanks[lankKind] = 0;
        }
    }

    /// <summary>
    /// ランクの種別ごとにランク情報を取得する
    /// </summary>
    /// <param name="lankKind"></param>
    /// <returns></returns>
    public int GetLanks(LankKindDef lankKind)
    {
        return lanks[lankKind];
    }

    /// <summary>
    /// ポイント情報をセットする
    /// </summary>
    /// <param name="pointKind"></param>
    /// <param name="value"></param>
    public void SetAddPoints(PointKindDef pointKind, int value)
    {
        points[pointKind] += value;
    }

    /// <summary>
    /// ポイント種別ごとにポイント情報を取得する
    /// </summary>
    /// <param name="pointKind"></param>
    /// <returns></returns>
    public int GetPoints(PointKindDef pointKind)
    {
        return points[pointKind];
    }

    /// <summary>
    /// ポイント情報を取得する
    /// </summary>
    /// <returns></returns>
    public Dictionary<PointKindDef, int> GetPoints()
    {
        return points;
    }

    /// <summary>
    /// ダイスボタンの表示を切り替える
    /// </summary>
    /// <param name="displayFlg"></param>
    public void SwitchDiceButtonDisplay(bool displayFlg)
    {
        diceButton.interactable = displayFlg;
    }

    /// <summary>
    /// プレイヤーがすでにゴールしていればボーナスを与える
    /// </summary>
    public void GetFinishedBonus()
    {
        if (lanks[LankKindDef.PartnerLank] == 0)
        {
            gameSceneManager.DisplayEventTextPanel("ゴールボーナス! 資金 +" + goalBonus);
        }
        else
        {
            gameSceneManager.DisplayEventTextPanel("ゴールボーナス! 資金 +" + goalBonus + '\n' +
                                                   "愛人からの献金 資金 +" + (3 * lanks[LankKindDef.PartnerLank]).ToString());
        }

        gameSceneManager.UpdatePlayerPanel(PointKindDef.MoneyPoints, 50 + (3 * lanks[LankKindDef.PartnerLank]));
    }

    /// <summary>
    /// ランク種別をenum型からstring型に変換
    /// </summary>
    /// <param name="lankKind"></param>
    /// <returns></returns>
    private string convertLankKind(LankKindDef lankKind)
    {
        string text = "";

        switch (lankKind)
        {
            case LankKindDef.BrotherLank:
                text = "舎弟";
                break;
            case LankKindDef.PartnerLank:
                text = "愛人";
                break;
        }

        return text;
    }

    /// <summary>
    /// ポイント種別をenum型からstring型に変換
    /// </summary>
    /// <param name="pointKind"></param>
    /// <returns></returns>
    private string convertPointKind(PointKindDef pointKind)
    {
        string text = "";

        switch (pointKind)
        {
            case PointKindDef.MoneyPoints:
                text = "資金";
                break;
            case PointKindDef.DancePoints:
                text = "ダンスP";
                break;
            case PointKindDef.PopularityPoints:
                text = "人望P";
                break;
            case PointKindDef.LovePoints:
                text = "恋愛P";
                break;
        }

        return text;
    }

    /// <summary>
    /// 数字を符号付数字(string型)に変換
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private string convertIntWithMark(int num)
    {
        string text = "";

        if (num > 0)
        {
            text = "+" + num;
        }
        else
        {
            text = num.ToString();
        }

        return text;
    }

    /// <summary>
    /// ダイスの目がゼロになるまで移動する
    /// </summary>
    /// <returns></returns>
    private IEnumerator walkToNextSquare()
    {
        walkingFlg = true;
        //フリーズを解除
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;

        Vector3 vec;
        Vector3 unitVec;

        while (restEyes != 0)
        {
            arrivedPosFlg = false;
            speed = 8.0f;
            
            //nextposに進む
            while (!arrivedPosFlg)
            {
                vec = nextPosition - gameObject.transform.position;
                unitVec = new Vector3(vec.x / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                      vec.y / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                      vec.z / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)));

                //次のマスの方向を向く
                this.transform.LookAt(nextPosition);

                rb.MovePosition(rb.position + speed * unitVec * Time.deltaTime);

                yield return null;
            }

            if (squareCollider.gameObject.CompareTag("Finish"))
            {
                FinishedFlg = true;
                restEyes = 0;
                break;
            }

            //次のマスの方向を向く
            this.transform.LookAt(nextPosition);

            restEyes--;

            //通過しようとしたマスが強制マス(試合マス)ならば
            if (squareCollider.GetComponent<Square>().IsForced)
            {
                restEyes = 0;
            }

            gameSceneManager.DisplayRestDiceEyes(restEyes);
        }

        //テキストオープン、テキスト表示
        SquareInfo squareInfo = squareCollider.gameObject.GetComponent<Square>().GetSquareInfo();
        string addText = "";

        //ランクアップデート
        if (squareInfo.lankPoint != 0)
        {
            gameSceneManager.UpdatePlayerPanel(squareInfo.lankKind, squareInfo.lankPoint);
            addText += '\n' + convertLankKind(squareInfo.lankKind) + " " + convertIntWithMark(squareInfo.lankPoint);
        }

        //ポイント種別分パネルアップデート
        for (int i = 0; i < squareInfo.pointKindList.Count; i++)
        {
            if (squareInfo.pointList[i] != 0)
            {
                gameSceneManager.UpdatePlayerPanel(squareInfo.pointKindList[i], squareInfo.pointList[i]);
                addText += '\n' + convertPointKind(squareInfo.pointKindList[i]) + " " + convertIntWithMark(squareInfo.pointList[i]);
            }
        }

        //愛人がいればターンの終わりに献金を得る
        if (lanks[LankKindDef.PartnerLank] > 0)
        {
            gameSceneManager.UpdatePlayerPanel(PointKindDef.MoneyPoints, donation * lanks[LankKindDef.PartnerLank]);
            addText += '\n' + "愛人からの献金 : 資金 " + convertIntWithMark(donation * lanks[LankKindDef.PartnerLank]); 
        }

        //止まったマスがショップマスならば
        if (squareInfo.squareKind == SquareKindDef.dualShop ||
            squareInfo.squareKind == SquareKindDef.brotherShop ||
            squareInfo.squareKind == SquareKindDef.partnerShop)
        {
            gameSceneManager.DisplayShopTextPanel(squareInfo.squareKind, squareInfo.eventText + addText);
        }
        //止まったマスが強制マス(試合マス)ならば
        else if (squareInfo.squareKind == SquareKindDef.forced)
        {
            gameSceneManager.DisplayCompeTextPanel(squareInfo.eventText + addText, points[PointKindDef.DancePoints]);
        }
        //それ以外のとき
        else
        {
            gameSceneManager.DisplayEventTextPanel(squareInfo.eventText + addText);
        }

        //止まったマスがワープマスならば
        if (squareInfo.squareKind == SquareKindDef.warp)
        {
            arrivedPosFlg = false;
            speed = 8.0f;

            //ワープ先に進む
            while (!arrivedPosFlg)
            {
                vec = warpPosition - gameObject.transform.position;
                unitVec = new Vector3(vec.x / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                      vec.y / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                      vec.z / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)));

                //次のマスの方向を向く
                this.transform.LookAt(warpPosition);

                rb.MovePosition(rb.position + speed * unitVec * Time.deltaTime);

                yield return null;
            }

            //次のマスの方向を向く
            this.transform.LookAt(nextPosition);

            gameSceneManager.DisplayRestDiceEyes(restEyes);
        }

        walkingFlg = false;
        speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        yield break;
    }

    /// <summary>
    /// トリガーコライダーにぶつかったときの処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other != squareCollider)
        {
            arrivedPosFlg = true;
            squareCollider = other;
            nextPosition = other.gameObject.GetComponent<Square>().NextPos;
            warpPosition = other.gameObject.GetComponent<Square>().WarpPos;
        }
    }
}
