using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobyCameraMove : MonoBehaviour
{
    [SerializeField]
    private LobbySceneManager lobbySceneManager;

    private GameObject targetObj;
    private Vector3 targetPos;

    void Start()
    {
        targetObj = lobbySceneManager.GetMyPlayerObject();
        targetPos = targetObj.transform.position;
        transform.position = new Vector3(targetPos.x + 5f,
                                         targetPos.y + 5f,
                                         targetPos.z + 5f);
    }

    void LateUpdate()
    {
        //targetの移動量分、自分も移動する
        transform.position += targetObj.transform.position - targetPos;
        targetPos = targetObj.transform.position;

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
    }
}
