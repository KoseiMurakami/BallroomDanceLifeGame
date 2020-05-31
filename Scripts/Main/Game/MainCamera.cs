using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager gameSceneManager = default;

    [SerializeField]
    private GameObject startPoint = default;

    [SerializeField]
    private float speed = default;

    private GameObject targetObj;
    private Vector3 defaultDistance;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool activeFlg = false;

    void Start()
    {
        transform.LookAt(startPoint.transform.position);
        StartCoroutine("gameOpenning");
        targetObj = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeFlg && targetObj != null)
        {
            this.targetPos = targetObj.transform.position;

            //常にtargetObjと適切な距離を保っておく
            transform.position = targetPos + defaultDistance;

            transform.LookAt(targetPos);
        }
    }

    private IEnumerator gameOpenning()
    {
        yield return new WaitForSeconds(2);
        startPos = startPoint.transform.position;
        float distance = 100;

        Vector3 vec = startPos - gameObject.transform.position;
        Vector3 unitVec = new Vector3(vec.x / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                      vec.y / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)),
                                      vec.z / Mathf.Sqrt((vec.x * vec.x + vec.y * vec.y + vec.z * vec.z)));

        while (distance > 20.0f)
        {
            transform.position += speed * unitVec * Time.deltaTime;
            distance = Vector3.Distance(startPos, transform.position);
            yield return null;
        }

        defaultDistance = transform.position - startPos;

        gameSceneManager.RequireActStart();

        activeFlg = true;

        yield break;
    }

    /*#======================================================================#*/
    /*#    function : SetTargetObj  function                                 #*/
    /*#    summary  : ターゲットオブジェクト情報をセットする                 #*/
    /*#    argument : (I)GameObject targetObj        -  ターゲット情報リスト #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SetTargetObj(GameObject targetObj)
    {
        this.targetObj = targetObj;
    }
}
