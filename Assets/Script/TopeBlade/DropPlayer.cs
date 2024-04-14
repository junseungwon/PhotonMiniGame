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
    //�÷��̾ �������� �� �߻�
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<TopBlade>())
        {
            system.JudgeWin(collision.gameObject);
        }
    }
}
 
