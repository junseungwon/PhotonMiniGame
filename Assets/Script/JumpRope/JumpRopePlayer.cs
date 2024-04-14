using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
public class JumpRopePlayer : MonoBehaviourPunCallbacks
{
    public float JumpeSpeed = 1000000f;

    private PhotonView pv;
    private Animator animator;
    private Rigidbody rb;
    private float runSpeed = 70f;
    private bool isJump = false;
    private bool isCheckJump = true;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }
    void Update()
    {
        Jump();
        TestMove();
        CheckFlow();
    }
    private void FixedUpdate()
    {
    }
    private void TestMove()//�Ϲ����� ���� ������
    {
        if (pv.IsMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 dir = new Vector3(h, 0, v);
            dir.Normalize();
            Debug.Log(isJump);
            if (dir != Vector3.zero&&isJump)
            {       
                animator.SetFloat("Run", 1.1f);
                if (Mathf.Sign(dir.x) != Mathf.Sign(transform.position.x) || Mathf.Sign(dir.z) != Mathf.Sign(transform.position.z))
                {
                    transform.Rotate(0, 1, 0);
                }
                transform.forward = Vector3.Lerp(transform.forward, dir, 100f * Time.deltaTime);
            }
            else
            {
                animator.SetFloat("Run", 0.9f);
            }
            rb.MovePosition(transform.position + dir * runSpeed * Time.deltaTime);

        }
    }
    //�÷��̾ �ȳѾ����� rigidBody Freezen�ص� 
    private void Jump()// ����ڰ� ����Ű ����
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isJump == true)
            {
                isCheckJump = false;
                isJump = false;
                Debug.Log("����Ű ����");
                animator.SetBool("Jump", true);
                StartCoroutine(StopAnimation("Jump", 1f));
            }
        }
    }
    //�ִϸ��̼� ���� �κ� 10�ʿ� �����ϵ��� ����
   public void AddForceJump()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * 30f, ForceMode.Force);
    }
    //���� �ִϸ��̼� ����
    private IEnumerator StopAnimation(string text, float time)//�ִϸ��̼��� ���� ������ false���� raycast ���� 2�� ���
    {
        yield return new WaitForSeconds(time);
        isCheckJump = true;
        animator.speed = 1.0f;
        animator.SetBool(text, false);
    }
    private void CheckFlow()//ĳ���Ͱ� �ٴڿ� ��Ҵ��� üũ
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.04f))
        {
            if (hit.collider.name == "Plane"&&isCheckJump== true)
            {
                isJump = true;
            }
            else
            {
                isJump = false;
            }
        }
        if (hit.collider == null) { isJump = false; }
    }
}
