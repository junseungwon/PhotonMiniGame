using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TopBladeSystem : MonoBehaviourPunCallbacks
{
    public Text[] scoreTexts = new Text[2];
    public Text[] nickNameText = new Text[2];
    public int[] playerScore = new int[2];
    private PhotonView pv = null;
    private int maxScore = 3;
    public Text moveTesting = null;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        NickNameSetting();
        StartCoroutine(MakeSetting());
    }
    private IEnumerator MakeSetting()
    {
        yield return new WaitForSeconds(5.0f);
        PhotonSystemManager.Instance.MakePlayer("TopBladePlayer", SetStartPoint(new Vector3(1, 6.36f, 1), new Vector3(-1, 6.36f, -1)));       
        pv.RPC(nameof(FindPlayers), RpcTarget.AllBuffered);
        pv.RPC(nameof(SetPlayerName), RpcTarget.AllBuffered);
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

    //생성된 팽이객체들의 이름을 초반 설정하기
    private void NickNameSetting()
    {
        nickNameText[(int)PublicEnum.Player.Master].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
        nickNameText[(int)PublicEnum.Player.Client].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
    }

    [PunRPC]
    private void ChangeTextPlayerScore(int num)//받아온 번호의 플레이어 점수표를 고치고 max바퀴를 돌면 승리자를 출력함.
    {
        ++playerScore[num];
        scoreTexts[num].text = playerScore[num].ToString();
        if (num == 0)
        {
            ResetPlayerPos((int)PublicEnum.Player.Client);
        }
        else
        {
            ResetPlayerPos((int)PublicEnum.Player.Master);
        }
        if (playerScore[num] == maxScore)
        {
            Debug.Log(PhotonNetwork.PlayerList[num].NickName + " 이 게임에서 승리 하셨습니다."); //3점이 넘으면 플레이어가 승리되었다고 나옴
            PhotonNetwork.LoadLevel((int)PublicEnum.ScenesEnum.BoardGameScene);
        }
    }

    //플레이어 위치 초기화
    private void ResetPlayerPos(int num)
    {
        PhotonSystemManager.Instance.players[num].transform.position = new Vector3(0, 6.36f, 0);
    }

    //떨어진 객체를 가져와서 떨어진 객체가 아닌 상대 객체의 점수를 향상시킨다.
    public void JudgeWin(GameObject obj)
    {
        //obj의 닉을 받아와서 플레이들을 비교한다. 해당 플레이어의 숫자를 받아서 해당 플레이어를 제외한 다른 플레이어에게 점수를 추가한다.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (obj.name != PhotonNetwork.PlayerList[i].NickName)
            {
                pv.RPC(nameof(ChangeTextPlayerScore), RpcTarget.AllBuffered, i);
                break;
            }
        }
    }
}
