using Photon.Pun;
using UnityEngine;

public class TopBlade : MonoBehaviourPunCallbacks
{
    public GameObject topBladeModel = null;
    public float speed = 100.0f;
    public float addSpeed = 1f; //가속도
    private Rigidbody rb;
    private PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rb.velocity.magnitude);
        TopBladeMove();
        GravityObject();
    }
    private void FixedUpdate()
    {
        RotationBlade();
    }

    //팽이 이동
    private void TopBladeMove()
    {
        if (pv.IsMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (h != 0 || v != 0)
            {
                if (addSpeed < 2.0f)
                {
                    addSpeed += 0.01f;
                }
                Vector3 dir = new Vector3(h, 0, v);
                rb.AddForce(dir * speed *addSpeed* Time.deltaTime, ForceMode.Impulse);
                FindObjectOfType<TopBladeSystem>().moveTesting.text = rb.velocity.magnitude.ToString();
            }
            else
            {
                addSpeed = 1f;
            }
        }
    }
    private void PlusAddForceTime()
    {

    }
    private void GravityObject()//중력
    {
        if (pv.IsMine)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
            {
                if (hit.collider.name != "Plane")
                {
                    Debug.Log("내려가는중");
                    rb.AddForce(Vector3.down * 30, ForceMode.Impulse);
                }
            }

        }
    }
    private void RotationBlade()//계속 회전하도록 rpc로 물체가 계속 회전하도록 제작
    {
        if (pv.IsMine)
        {
            topBladeModel.transform.Rotate(Vector3.down * 10.0f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (pv.IsMine)
        {
            if (collision.gameObject.tag == "Player")
            {
                float OperV = collision.gameObject.transform.GetComponent<Rigidbody>().velocity.magnitude;
                float MineV = rb.velocity.magnitude;
                Debug.Log(OperV+" "+ MineV);
                //나 - 상대
                Vector3 moveV = transform.position - collision.gameObject.transform.position;
                if (OperV > MineV)
                {
                    Debug.Log("상대가 더 세게 박음");
                    rb.AddForce(moveV * 3000 * Time.deltaTime, ForceMode.Force);
                }
                else
                {
                    Debug.Log("내가 더 세게 박음");
                    rb.AddForce(moveV * 10 * Time.deltaTime, ForceMode.Force);
                }
            }
        }
    }
}
