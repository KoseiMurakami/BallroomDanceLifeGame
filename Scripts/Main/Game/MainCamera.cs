using System.Collections;
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
    private Vector3 defaultDistance = new Vector3(7.7f, 13.0f, 12.2f);
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool activeFlg = false;

    void Start()
    {
        transform.LookAt(startPoint.transform.position);
        StartCoroutine("gameOpenning");
        targetObj = startPoint;
    }

    // LateUptateでカメラのブレ予防
    void LateUpdate()
    {
        if (activeFlg && targetObj != null)
        {
            //1f後のターゲットオプションオフセット
            Vector3 offset = targetObj.transform.position - targetPos;

            transform.position += offset;

            this.transform.LookAt(targetPos);

            //マウスの右クリックを押している間
            if (Input.GetMouseButton(1))
            {
                //マウスの移動量
                float mouseInputX = Input.GetAxis("Mouse X");
                float mouseInputY = Input.GetAxis("Mouse Y");

                //targetの位置のy軸を中心に回転(公転)する
                transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * 200f);
                //カメラの垂直移動
                transform.RotateAround(targetPos, transform.right, -mouseInputY * Time.deltaTime * 200f);
            }

            //1f前のターゲットポジションを取得しておく
            this.targetPos = targetObj.transform.position;

            //ターンごとにアングル変わるとだるいのでここでデフォルト登録
            defaultDistance = transform.position - targetPos;
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
        activeFlg = true;
        this.targetObj = targetObj;
        this.targetPos = targetObj.transform.position;
        transform.position = targetPos + defaultDistance;
    }
}
