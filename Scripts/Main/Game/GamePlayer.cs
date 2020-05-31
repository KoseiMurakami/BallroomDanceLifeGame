using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayer : MonoBehaviour
{
    public int PlayerId { set; get; }
    public string PlayerName { set; get; }

    private float speed;

    private GameSceneManager gameSceneManager;
    static private Button diceButton;
    static private Text eyesText;
    private Animator animator;
    private Rigidbody rb;
    private Collider squareCollider = new Collider();

    private bool arrivedPosFlg = true;
    private bool walkingFlg = false;
    private bool finishedFlg = false;
    private int restEyes = 0;
    private Vector3 nextPosition;

    void Start()
    {
        Debug.Log("GamePlayerがインスタンス化されました。");

        gameSceneManager = 
            GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        diceButton = GameObject.Find("DiceButton").GetComponent<Button>();
        eyesText = GameObject.Find("DiceEyesText").GetComponent<Text>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        speed = 0;
        eyesText.text = "0";

        //次のマスの方向を向く
        this.transform.LookAt(nextPosition);

        SwitchDiceButtonDisplay(false);
    }

    void Update()
    {
        animator.SetFloat("speed", speed);

        if (restEyes > 0 && !walkingFlg && !finishedFlg)
        {
            Debug.Log("コルーチンスタート");
            StartCoroutine("walkToNextSquare");
        }
    }


    public void setTheDice(int eyes)
    {
        restEyes = eyes;
        SwitchDiceButtonDisplay(false);
    }

    /*#======================================================================#*/
    /*#    function : SwitchDiceButtonDisplay                                #*/
    /*#    summary  : ダイスボタン表示を切り替える                           #*/
    /*#    argument : bool  displayFlg                -  表示/非表示フラグ   #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SwitchDiceButtonDisplay(bool displayFlg)
    {
        diceButton.interactable = displayFlg;
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : waldToNextSquare  coroutine                            #*/
    /*#    summary  : さいころの目がゼロになるまで移動する                   #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
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
            vec = nextPosition - gameObject.transform.position;
            unitVec = new Vector3(vec.x / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                  vec.y / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                  vec.z / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)));

            //nextposに進む
            while (!arrivedPosFlg)
            {
                //次のマスの方向を向く
                this.transform.LookAt(nextPosition);

                rb.MovePosition(rb.position + speed * unitVec * Time.deltaTime);
                yield return null;
            }

            if (squareCollider.gameObject.tag == "Finish")
            {
                finishedFlg = true;
                break;
            }

            //次のマスの方向を向く
            this.transform.LookAt(nextPosition);

            restEyes--;
            gameSceneManager.DisplayRestDiceEyes(restEyes);
        }

        rb.constraints = RigidbodyConstraints.FreezeAll;

        speed = 0;

        //テキストオープン、テキスト表示
        string eventText = squareCollider.gameObject.GetComponent<Square>().GetEventText();
        gameSceneManager.DisplayEventTextPanel(eventText);
        walkingFlg = false;

        yield break;
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : OnTriggerEnter  function                               #*/
    /*#    summary  : オブジェクトがTriggerコライダーに入ったら処理を行う    #*/
    /*#    argument : Collider  other                 -  Triggerコライダー   #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    private void OnTriggerEnter(Collider other)
    {
        if (other != squareCollider)
        {
            arrivedPosFlg = true;
            squareCollider = other;
            nextPosition = other.gameObject.GetComponent<Square>().GetNextSquarePos();
        }
    }
}
