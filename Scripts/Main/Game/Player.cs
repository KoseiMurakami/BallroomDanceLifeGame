using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    private float speed;

    private GameSceneManager gameSceneManager;
    private Animator animator;
    private Rigidbody rb;
    private Collider squareCollider = new Collider();

    private bool WalkingFlg = false;
    private bool arrivedPosFlg = true;
    private int restEyes = 0;
    private Vector3 nextPosition;

    void Start()
    {
        //自分自身が生成していないオブジェクトのコンポーネントを削除
        if (!photonView.IsMine)
        {
            Destroy(this);
        }

        gameSceneManager = 
            GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        speed = 0;

        //次のマスの方向を向く
        this.transform.LookAt(nextPosition);
    }

    void Update()
    {
        if (restEyes > 0 && !WalkingFlg)
        {
            StartCoroutine("walkToNextSquare");
        }

        animator.SetFloat("speed", speed);
    }

    /*#======================================================================#*/
    /*#    function : SetDiceEyes  function                                  #*/
    /*#    summary  : さいころの出目をセットする                             #*/
    /*#    argument : int  eyes                       -  さいころの目        #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void GetDiceEyes(int eyes)
    {
        restEyes = eyes;
    }

    /*#----------------------------------------------------------------------#*/
    /*#    function : waldToNextSquare  coroutine                            #*/
    /*#    summary  : さいころの目がゼロになるまで移動する                   #*/
    /*#    argument : nothing                                                #*/
    /*#    return   : nothing                                                #*/
    /*#----------------------------------------------------------------------#*/
    private IEnumerator walkToNextSquare()
    {
        WalkingFlg = true;
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

            //次のマスの方向を向く
            this.transform.LookAt(nextPosition);

            restEyes--;
        }

        rb.constraints = RigidbodyConstraints.FreezeAll;

        WalkingFlg = false;
        speed = 0;

        //テキストオープン、テキスト表示
        string eventText = squareCollider.gameObject.GetComponent<Square>().GetEventText();
        gameSceneManager.DisplayEventTextPanel(eventText);

        yield return new WaitForSeconds(3);

        gameSceneManager.ReplyActEnd();

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
        if (other.gameObject.tag == "Square")
        {
            arrivedPosFlg = true;
            squareCollider = other;
            nextPosition = other.gameObject.GetComponent<Square>().GetNextSquarePos();
        }

        if (other.gameObject.tag == "Finish")
        {
            arrivedPosFlg = true;
            Debug.Log("ゴールしました！");
        }
    }
}
