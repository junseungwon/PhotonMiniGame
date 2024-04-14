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
    private bool isBooster = false; //�ν��Ͱ� Ȱ��ȭ ����
    private bool isMove = true; //������ �� �ִ� ����
    private float boostertime = 0f; //�ν��� �ð�
    private float limitSpeed = 1.0f; //���Ѽӵ�
    private float breakSpeed = 1.0f; // �극��ũ �ӵ�
    private float boosterSpeed = 1.0f; //�ν��� �ӵ�

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
    private void CartMove()//�ڵ��� ���� ������ ����
    {
        if (pv.IsMine)
        {
            BreakPedal();
            CartController();
        }

    }
    //���� �ٴڿ� �ν������� üũ����Ʈ���� Ȯ���ϱ�
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
            if (hit.tag == "CheckPoint") //raycast �浹�� üũ����Ʈ�� ���
            {
                int num = int.Parse(hit.name.Substring(hit.name.Length - 1)); //�浹��ü�� ��ȣ�� �޾ƿ�
                if (num == 0)
                {
                    isCheckPoint[num] = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (isCheckPoint[i] == false) //�������� üũ����Ʈ�� ������ ���
                        {
                            break;
                        }
                        if (i == 3) //3�� üũ����Ʈ���� �ٵ����� ��� ���� 1�� �ö��
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
    //üũ����Ʈ �ʱ�ȭ
    private void ResetIsCheckPoint()
    {
        for (int i = 0; i < isCheckPoint.Length; i++)
        {
            isCheckPoint[i] = false;
        }
    }
    private void CheckBooster(GameObject hits)//�ν��� ���� üũ
    {
        if (hits.name == "Booster")
        {
            if (isBooster) //�ν��͸� �ӵ��� 5���
            {
                boostertime = 0f;
                boosterSpeed = 5.0f;
            }
            else
            {
                keepingBoosterTime = KeepBoosterSpeed();
                StartCoroutine(keepingBoosterTime); //�ڷ�ƾ���� ���� �ν��� �ӵ� ����
            }
        }
    }
    private IEnumerator KeepBoosterSpeed()//�ν��� ���ӽð�
    {
        isBooster = true;
        boostertime = 0f;
        boosterSpeed = 5.0f;
        while (boosterSpeed > 1f || boostertime > 5.0f)
        {
            boostertime += 0.1f;
            boosterSpeed -= 0.3f;
            Debug.Log("�ν��� �����");
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("�ν��� ��볡");
        isBooster = false;
    }
    private void BreakPedal()//�극��ũ
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
    private void CartController()//īƮ �̵�
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if ((x != 0 || z != 0) && isMove != false)
        {
            limitSpeed += Time.deltaTime;

            if (limitSpeed > 3.0f) { isMove = false; limitSpeed = 0; StartCoroutine(StopMove()); } //���Ѽӵ��� ������ �ӵ� �ʱ�ȭ
            rb.angularVelocity = new Vector3(0, x * 2f, 0); //īƮȸ��
            rb.AddForce(transform.forward * 20f * z * boosterSpeed, ForceMode.Force); //īƮ �̵�

            //Debug.Log(limitSpeed);
            if (limitSpeed > 3.0f) { isMove = false; limitSpeed = 0; StartCoroutine(StopMove()); }
            rb.angularVelocity = new Vector3(0, x * 2f, 0);
            rb.AddForce(transform.forward * 20f * z * boosterSpeed*Time.deltaTime, ForceMode.Force);

        }
    }
    private IEnumerator StopMove()//addforce�ʹ� �����ϸ� �ʹ� ��������
    {
        Debug.Log("��� ����");
        yield return null;
        isMove = true;
    }
}
