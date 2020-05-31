using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LobbyPlayer : MonoBehaviourPunCallbacks
{
    private float moveSpeed = 3.0f;
    float inputHorizontal;
    float inputVertical;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 m_LastPosition = new Vector3();

    void Start()
    {
        Debug.Log("LobbyPlayerがインスタンス化されました。");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
            moveSpeed = 3.0f;

            if (Input.GetKey(KeyCode.Space))
            {
                moveSpeed = 10f;
            }
        }
    }

    private void FixedUpdate()
    {
        //カメラの方向からxz平面の単位ベクトルを取得
        Vector3 cameraForward =
            Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        //方向キーの入力値とカメラの向きから移動方向を決定
        Vector3 moveForward =
            cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;

        //移動方向にスピードを掛ける
        rb.velocity = moveForward * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        //キャラクターの向きを進行方向にする
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveForward);
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        Vector3 movementVector = transform.position - m_LastPosition;

        float animspeed = Vector3.Dot(movementVector.normalized, transform.forward);

        animator.SetFloat("speed", moveSpeed * animspeed);

        m_LastPosition = transform.position;
    }

}
