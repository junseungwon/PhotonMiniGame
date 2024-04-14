using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPlayer : MonoBehaviour
{
    private TopBladeSystem system;
    private void Start()
    {
        system = FindObjectOfType<TopBladeSystem>();
    }
    //플레이어가 떨어졌을 때 발생
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<TopBlade>())
        {
            system.JudgeWin(collision.gameObject);
        }
    }
}
 
