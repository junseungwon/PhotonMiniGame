using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class JumpRope : MonoBehaviourPunCallbacks
{
    public int[] playerScore = new int[2]; //플레이어 점수
    public Text[] scoreTexts = new Text[2]; //플레이어 점수text
    public Text[] nickNameText = new Text[2]; //플레이어 닉네임 text

    [SerializeField] private GameObject rope = null; // 줄넘기 줄
    [SerializeField] private GameObject centerPos = null; //줄 회전 중심

    private PhotonView pv = null;
    private Vector3 startPos = Vector3.zero;
    private float ropeSpeed = 20f; //줄속도
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


    //닉넴text이름바꾸기
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
    //플레이어위치 초반 세팅
    private Vector3 SetStartPoint(Vector3 v1, Vector3 v2)//초기위치 설정
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
    [PunRPC] //플레이어들을 찾아서 배열에 넣어라
    private void FindPlayers()
    {
        PhotonSystemManager.Instance.players = GameObject.FindGameObjectsWithTag("Player");
    }

    //플레이어가 모두 만들어지면 물체들의 닉넴을 변경한다.
    [PunRPC]
    private void SetPlayerName()
    {
        if (PhotonSystemManager.Instance.players.Length == 2)
        {
            PhotonSystemManager.Instance.players[0].name = PhotonNetwork.PlayerList[0].NickName;
            PhotonSystemManager.Instance.players[1].name = PhotonNetwork.PlayerList[1].NickName;
        }
    }
    //줄넘기와 플레이어가 충돌 시 승자를 판단
    public void JudgeWin(GameObject obj)
    {
        //obj의 닉을 받아와서 플레이들을 비교한다. 해당 플레이어의 숫자를 받아서 해당 플레이어를 제외한 다른 플레이어에게 점수를 추가한다.
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
    private void ChangeScore(int num)//승리자 번호를 num으로 받아와서 점수 상승
    {
        ++playerScore[num];
        scoreTexts[num].text = playerScore[num].ToString();
        StopRotationRope();
        RopeReSetting();
    }
    private void RotationRope()//동아줄 회전
    {
        if (isRotation)
        {
            rope.transform.RotateAround(centerPos.transform.position, Vector3.left, ropeSpeed * Time.deltaTime);
        }
    }
    private void StopRotationRope()//줄 회전을 멈춤
    {
        isRotation = false;
        rope.transform.position = new Vector3(0, 6.8f, 0);
    }
    private void RopeReSetting()// 줄 재세팅
    {
        StartCoroutine(StartInN(5f));
    }
    private IEnumerator StartInN(float time)//n초 후 시작
    {
        yield return new WaitForSeconds(time);
        isRotation = true;
        Debug.Log("gamestart");
    }
}
