using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerCart : MonoBehaviour
{
    public GameObject checkRayder = null;
    public bool[] isCheckPoint = new bool[4];
    public float raceMoveSpeed = 10.0f;
    public float raceRotationSpeed = 1.5f;


    private IEnumerator keepingBoosterTime;
    private PhotonView pv;
    private Rigidbody rb;
    private bool isBooster = false; //부스터가 활성화 상태
    private bool isMove = true; //움직일 수 있는 상태
    private float boostertime = 0f; //부스터 시간
    private float limitSpeed = 1.0f; //제한속도
    private float breakSpeed = 1.0f; // 브레이크 속도
    private float boosterSpeed = 1.0f; //부스터 속도

    // Start is called before the first frame update
    void Start()
    {

        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRayCast();

    }
    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            CartMove();
        }

    }
    private void CartMove()//자동차 종합 움직임 실행
    {
        if (pv.IsMine)
        {
            BreakPedal();
            CartController();
        }

    }
    //현재 바닥에 부스터인지 체크포인트인지 확인하기
    private void CheckRayCast()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(checkRayder.transform.position, Vector3.down, 10f);
        for (int i = 0; i < hits.Length; i++)
        {
            CheckBooster(hits[i].collider.gameObject);
            IsCheckPoint(hits[i].collider.gameObject);
        }

    }

    private void IsCheckPoint(GameObject hit)
    {
        if (pv.IsMine)
        {
            if (hit.tag == "CheckPoint") //raycast 충돌이 체크포인트일 경우
            {
                int num = int.Parse(hit.name.Substring(hit.name.Length - 1)); //충돌객체의 번호를 받아옴
                if (num == 0)
                {
                    isCheckPoint[num] = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (isCheckPoint[i] == false) //돌지않은 체크포인트가 있으면 취소
                        {
                            break;
                        }
                        if (i == 3) //3번 체크포인트까지 다돌았을 경우 점수 1이 올라옴
                        {
                            FindObjectOfType<RaceCartSystem>().OneMoreLap();
                            ResetIsCheckPoint();
                        }
                    }
                }
                else
                {
                    if (isCheckPoint[num - 1] == true)
                    {
                        isCheckPoint[num] = true;
                    }
                }
                isCheckPoint[num] = true;
            }
        }
    }
    //체크포인트 초기화
    private void ResetIsCheckPoint()
    {
        for (int i = 0; i < isCheckPoint.Length; i++)
        {
            isCheckPoint[i] = false;
        }
    }
    private void CheckBooster(GameObject hits)//부스터 발판 체크
    {
        if (hits.name == "Booster")
        {
            if (isBooster) //부스터면 속도를 5상승
            {
                boostertime = 0f;
                boosterSpeed = 5.0f;
            }
            else
            {
                keepingBoosterTime = KeepBoosterSpeed();
                StartCoroutine(keepingBoosterTime); //코루틴으로 점차 부스터 속도 감소
            }
        }
    }
    private IEnumerator KeepBoosterSpeed()//부스터 지속시간
    {
        isBooster = true;
        boostertime = 0f;
        boosterSpeed = 5.0f;
        while (boosterSpeed > 1f || boostertime > 5.0f)
        {
            boostertime += 0.1f;
            boosterSpeed -= 0.3f;
            Debug.Log("부스터 사용중");
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("부스터 사용끝");
        isBooster = false;
    }
    private void BreakPedal()//브레이크
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            breakSpeed += Time.deltaTime;
            if (breakSpeed > 3.0f)
            {
                breakSpeed = 3.0f;
            }
            rb.mass = breakSpeed;
        }
        else
        {
            breakSpeed = 1.0f;
            rb.mass = breakSpeed;
        }

    }
    private void CartController()//카트 이동
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if ((x != 0 || z != 0) && isMove != false)
        {
            limitSpeed += Time.deltaTime;

            if (limitSpeed > 3.0f) { isMove = false; limitSpeed = 0; StartCoroutine(StopMove()); } //제한속도가 넘으면 속도 초기화
            rb.angularVelocity = new Vector3(0, x * 2f, 0); //카트회전
            rb.AddForce(transform.forward * 20f * z * boosterSpeed, ForceMode.Force); //카트 이동

            //Debug.Log(limitSpeed);
            if (limitSpeed > 3.0f) { isMove = false; limitSpeed = 0; StartCoroutine(StopMove()); }
            rb.angularVelocity = new Vector3(0, x * 2f, 0);
            rb.AddForce(transform.forward * 20f * z * boosterSpeed*Time.deltaTime, ForceMode.Force);

        }
    }
    private IEnumerator StopMove()//addforce너무 많이하면 너무 무빡구임
    {
        Debug.Log("잠깐 멈춤");
        yield return null;
        isMove = true;
    }
}
