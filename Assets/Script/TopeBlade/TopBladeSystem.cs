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

    //������ ���̰�ü���� �̸��� �ʹ� �����ϱ�
    private void NickNameSetting()
    {
        nickNameText[(int)PublicEnum.Player.Master].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
        nickNameText[(int)PublicEnum.Player.Client].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
    }

    [PunRPC]
    private void ChangeTextPlayerScore(int num)//�޾ƿ� ��ȣ�� �÷��̾� ����ǥ�� ��ġ�� max������ ���� �¸��ڸ� �����.
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
            Debug.Log(PhotonNetwork.PlayerList[num].NickName + " �� ���ӿ��� �¸� �ϼ̽��ϴ�."); //3���� ������ �÷��̾ �¸��Ǿ��ٰ� ����
            PhotonNetwork.LoadLevel((int)PublicEnum.ScenesEnum.BoardGameScene);
        }
    }

    //�÷��̾� ��ġ �ʱ�ȭ
    private void ResetPlayerPos(int num)
    {
        PhotonSystemManager.Instance.players[num].transform.position = new Vector3(0, 6.36f, 0);
    }

    //������ ��ü�� �����ͼ� ������ ��ü�� �ƴ� ��� ��ü�� ������ ����Ų��.
    public void JudgeWin(GameObject obj)
    {
        //obj�� ���� �޾ƿͼ� �÷��̵��� ���Ѵ�. �ش� �÷��̾��� ���ڸ� �޾Ƽ� �ش� �÷��̾ ������ �ٸ� �÷��̾�� ������ �߰��Ѵ�.
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
