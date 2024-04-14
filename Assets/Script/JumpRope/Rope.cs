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

    //줄넘기와 플레이어 충돌 시에 발생
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<JumpRopePlayer>())
        {
            Debug.Log(other.gameObject.name);
            jumpRope.JudgeWin(other.gameObject);
        }
    }
}
