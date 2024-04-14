using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private JumpRope jumpRope;
    private void Start()
    {
        jumpRope = FindObjectOfType<JumpRope>();
    }

    //�ٳѱ�� �÷��̾� �浹 �ÿ� �߻�
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<JumpRopePlayer>())
        {
            Debug.Log(other.gameObject.name);
            jumpRope.JudgeWin(other.gameObject);
        }
    }
}
