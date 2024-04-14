using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class JumpRope : MonoBehaviourPunCallbacks
{
    public int[] playerScore = new int[2]; //�÷��̾� ����
    public Text[] scoreTexts = new Text[2]; //�÷��̾� ����text
    public Text[] nickNameText = new Text[2]; //�÷��̾� �г��� text

    [SerializeField] private GameObject rope = null; // �ٳѱ� ��
    [SerializeField] private GameObject centerPos = null; //�� ȸ�� �߽�

    private PhotonView pv = null;
    private Vector3 startPos = Vector3.zero;
    private float ropeSpeed = 20f; //�ټӵ�
    private bool isRotation = false;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        startPos = rope.transform.position;
        StartCoroutine(MakeSetting());
        NickNameSetting();
    }

    // Update is called once per frame
    void Update()
    {
        RotationRope();
    }


    //�г�text�̸��ٲٱ�
    private void NickNameSetting()
    {
        nickNameText[(int)PublicEnum.Player.Master].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
        nickNameText[(int)PublicEnum.Player.Client].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
    }
    private IEnumerator MakeSetting()
    {
        yield return new WaitForSeconds(2.0f);
        PhotonSystemManager.Instance.MakePlayer("JumpRopePlayer", SetStartPoint(new Vector3(1, 0.3f, 0), new Vector3(-1, 0.3f, 0)));
        pv.RPC(nameof(FindPlayers), RpcTarget.AllBuffered);
        pv.RPC(nameof(SetPlayerName), RpcTarget.AllBuffered);
        RopeReSetting();
    }
    //�÷��̾���ġ �ʹ� ����
    private Vector3 SetStartPoint(Vector3 v1, Vector3 v2)//�ʱ���ġ ����
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return v1;
        }
        else
        {
            return v2;
        }
    }
    [PunRPC] //�÷��̾���� ã�Ƽ� �迭�� �־��
    private void FindPlayers()
    {
        PhotonSystemManager.Instance.players = GameObject.FindGameObjectsWithTag("Player");
    }

    //�÷��̾ ��� ��������� ��ü���� �г��� �����Ѵ�.
    [PunRPC]
    private void SetPlayerName()
    {
        if (PhotonSystemManager.Instance.players.Length == 2)
        {
            PhotonSystemManager.Instance.players[0].name = PhotonNetwork.PlayerList[0].NickName;
            PhotonSystemManager.Instance.players[1].name = PhotonNetwork.PlayerList[1].NickName;
        }
    }
    //�ٳѱ�� �÷��̾ �浹 �� ���ڸ� �Ǵ�
    public void JudgeWin(GameObject obj)
    {
        //obj�� ���� �޾ƿͼ� �÷��̵��� ���Ѵ�. �ش� �÷��̾��� ���ڸ� �޾Ƽ� �ش� �÷��̾ ������ �ٸ� �÷��̾�� ������ �߰��Ѵ�.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (obj.name != PhotonNetwork.PlayerList[i].NickName)
            {
                pv.RPC(nameof(ChangeScore), RpcTarget.AllBuffered, i);
                break;
            }
        }
    }

    [PunRPC]
    private void ChangeScore(int num)//�¸��� ��ȣ�� num���� �޾ƿͼ� ���� ���
    {
        ++playerScore[num];
        scoreTexts[num].text = playerScore[num].ToString();
        StopRotationRope();
        RopeReSetting();
    }
    private void RotationRope()//������ ȸ��
    {
        if (isRotation)
        {
            rope.transform.RotateAround(centerPos.transform.position, Vector3.left, ropeSpeed * Time.deltaTime);
        }
    }
    private void StopRotationRope()//�� ȸ���� ����
    {
        isRotation = false;
        rope.transform.position = new Vector3(0, 6.8f, 0);
    }
    private void RopeReSetting()// �� �缼��
    {
        StartCoroutine(StartInN(5f));
    }
    private IEnumerator StartInN(float time)//n�� �� ����
    {
        yield return new WaitForSeconds(time);
        isRotation = true;
        Debug.Log("gamestart");
    }
}
