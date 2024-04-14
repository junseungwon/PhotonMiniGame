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
    private void TestMove()//일반적인 조작 움직임
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
    //플레이어가 안넘어지게 rigidBody Freezen해둠 
    private void Jump()// 사용자가 점프키 누름
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isJump == true)
            {
                isCheckJump = false;
                isJump = false;
                Debug.Log("점프키 눌림");
                animator.SetBool("Jump", true);
                StartCoroutine(StopAnimation("Jump", 1f));
            }
        }
    }
    //애니메이션 점프 부분 10초에 점프하도록 지시
   public void AddForceJump()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * 30f, ForceMode.Force);
    }
    //점프 애니메이션 종료
    private IEnumerator StopAnimation(string text, float time)//애니메이션을 몇초 지정후 false변경 raycast 오류 2번 재생
    {
        yield return new WaitForSeconds(time);
        isCheckJump = true;
        animator.speed = 1.0f;
        animator.SetBool(text, false);
    }
    private void CheckFlow()//캐릭터가 바닥에 닿았는지 체크
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
