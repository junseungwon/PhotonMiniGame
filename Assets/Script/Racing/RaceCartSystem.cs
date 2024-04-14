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
    private int maxLap = 3; //�ִ������ test������ 1�� �ص�
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        NickNameSetting();
        StartCoroutine(MakeSetting());
    }

    //�÷��̾� text�̸� �ʹ� ����
    private void NickNameSetting()
    {
        nickNameText[(int)PublicEnum.Player.Master].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
        nickNameText[(int)PublicEnum.Player.Client].text = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
    }
    //�÷��̾� ��ȯ�ϰ� ����
    private IEnumerator MakeSetting()
    {
        yield return new WaitForSeconds(3f);
        PhotonSystemManager.Instance.MakePlayer("CartPlayer", SetStartPoint(new Vector3(-13, 0.3f, -135), new Vector3(-13, 0.3f, -160)));
        pv.RPC(nameof(FindPlayers), RpcTarget.AllBuffered);
        pv.RPC(nameof(SetPlayerName), RpcTarget.AllBuffered);
        yield return new WaitForSeconds(1.0f);
        PlayerCameraSetting();
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
            PhotonSystemManager.Instance.players[(int)PublicEnum.Player.Master].name = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName;
            PhotonSystemManager.Instance.players[(int)PublicEnum.Player.Client].name = PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName;
        }
    }
    [PunRPC]
    private void ChangeTextPlayerLap(int num)//�޾ƿ� ��ȣ�� �÷��̾� ����ǥ�� ��ġ�� max������ ���� �¸��ڸ� �����.
    {
        playerLap[num]++;
        scoreTexts[num].text = playerLap[num].ToString();
        if (playerLap[num] == maxLap)
        {
            Debug.Log(PhotonNetwork.PlayerList[num].NickName + " �� ���ӿ��� �¸� �ϼ̽��ϴ�.");
            PhotonNetwork.LoadLevel((int)PublicEnum.ScenesEnum.BoardGameScene);
        }
    }
    //�÷��̾ 1������ ������ �� �ش� �÷��̾ ������ �ý��۰� �������ش�.
    public void OneMoreLap()
    {
        pv.RPC(nameof(ChangeTextPlayerLap), RpcTarget.AllBuffered, PhotonSystemManager.Instance.ReturnNumForPhotonViewIsMine());
    }
    //�÷��̾� ī�޶� ����
    private void PlayerCameraSetting()
    {
        Camera.main.gameObject.SetActive(false);
        PhotonSystemManager.Instance.players[PhotonSystemManager.Instance.ReturnNumForPhotonViewIsMine()].transform.GetChild(1).gameObject.SetActive(true);
    }
}
