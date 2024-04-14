using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class RaceCartSystem : MonoBehaviourPunCallbacks
{
    public Text[] scoreTexts = new Text[2];
    public Text[] nickNameText = new Text[2];
    public int[] playerLap = new int[2];
    private PhotonView pv = null;
    private int maxLap = 3; //최대바퀴수 test용으로 1로 해둠
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        NickNameSetting();
        StartCoroutine(MakeSetting());
    }

    //플레이어 text이름 초반 세팅
    private void NickNameSetting()
    {
        nickNameText[(int)PublicEnum.Player.Master].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
        nickNameText[(int)PublicEnum.Player.Client].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
    }
    //플레이어 소환하고 세팅
    private IEnumerator MakeSetting()
    {
        yield return new WaitForSeconds(3f);
        PhotonSystemManager.Instance.MakePlayer("CartPlayer", SetStartPoint(new Vector3(-13, 0.3f, -135), new Vector3(-13, 0.3f, -160)));
        pv.RPC(nameof(FindPlayers), RpcTarget.AllBuffered);
        pv.RPC(nameof(SetPlayerName), RpcTarget.AllBuffered);
        yield return new WaitForSeconds(1.0f);
        PlayerCameraSetting();
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
            PhotonSystemManager.Instance.players[(int)PublicEnum.Player.Master].name = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
            PhotonSystemManager.Instance.players[(int)PublicEnum.Player.Client].name = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
        }
    }
    [PunRPC]
    private void ChangeTextPlayerLap(int num)//받아온 번호의 플레이어 점수표를 고치고 max바퀴를 돌면 승리자를 출력함.
    {
        playerLap[num]++;
        scoreTexts[num].text = playerLap[num].ToString();
        if (playerLap[num] == maxLap)
        {
            Debug.Log(PhotonNetwork.PlayerList[num].NickName + " 이 게임에서 승리 하셨습니다.");
            PhotonNetwork.LoadLevel((int)PublicEnum.ScenesEnum.BoardGameScene);
        }
    }
    //플레이어가 1바퀴를 돌았을 때 해당 플레이어를 점수판 시스템과 연결해준다.
    public void OneMoreLap()
    {
        pv.RPC(nameof(ChangeTextPlayerLap), RpcTarget.AllBuffered, PhotonSystemManager.Instance.ReturnNumForPhotonViewIsMine());
    }
    //플레이어 카메라 세팅
    private void PlayerCameraSetting()
    {
        Camera.main.gameObject.SetActive(false);
        PhotonSystemManager.Instance.players[PhotonSystemManager.Instance.ReturnNumForPhotonViewIsMine()].transform.GetChild(1).gameObject.SetActive(true);
    }
}
