using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float speed = default;

    private GameSceneManager gameSceneManager;
    private Vector3 nextPosition;
    private Collider squareCollider = new Collider();
    private int restEyes = 0;
    private bool WalkingFlg = false;
    private bool arrivedPosFlg = true;

    void Start()
    {
        //自分自身が生成していないオブジェクトのコンポーネントを削除
        if (!photonView.IsMine)
        {
            Destroy(this);
        }

        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    void Update()
    {
        if (restEyes > 0 && !WalkingFlg)
        {
            StartCoroutine("walkToNextSquare");
        }
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
        Vector3 vec;
        Vector3 unitVec;

        while (restEyes != 0)
        {
            arrivedPosFlg = false;
            vec = nextPosition - gameObject.transform.position;
            unitVec = new Vector3(vec.x / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                  vec.y / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                  vec.z / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)));
            //nextposに進む
            while (!arrivedPosFlg)
            {
                gameObject.transform.position += new Vector3(speed * unitVec.x * Time.deltaTime,
                                                             speed * unitVec.y * Time.deltaTime,
                                                             speed * unitVec.z * Time.deltaTime); 
                yield return null;
            }

            restEyes--;
        }

        WalkingFlg = false;

        //テキストオープン、テキスト表示
        string eventText = squareCollider.gameObject.GetComponent<Square>().GetEventText();
        gameSceneManager.DisplayEventTextPanel(eventText);

        yield return new WaitForSeconds(3);

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
        arrivedPosFlg = true;
        squareCollider = other;
        nextPosition = other.gameObject.GetComponent<Square>().GetNextSquarePos();
    }
}
