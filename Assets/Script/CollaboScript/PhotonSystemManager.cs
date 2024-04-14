using Photon.Pun;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Text = UnityEngine.UI.Text;

public class PhotonSystemManager : MonoBehaviourPunCallbacks
{
    private static PhotonSystemManager instance = null;
    public static PhotonSystemManager Instance
    {
        get { if (instance == null) instance = FindObjectOfType<PhotonSystemManager>(); return instance; }
    }
    public GameObject[] players = new GameObject[2]; 
   
    [SerializeField] private Text nickNameText = null;
    private string nickName = "";
    private PhotonView pv;

    private void Awake()//ó�� ����
    {
        pv = GetComponent<PhotonView>();
        DontDestroyOnLoad(this);
        Screen.SetResolution(1200, 1000, false);
    }
    //����ȭ�鿡�� ������ ������ ������Ӿ����� �̵��Ѵ�.
    //1��1�� �ؼ� ���� ���� ���� ������ �ȴ�.
    public void GoToBoardGameScence()
    {
        GetNickName();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LoadLevel(1);       
    }
    //�ٳѱ� , īƮ , ���� ���� ������ �̵��ϱ� ��ư��
    [PunRPC]
    private void ChangeLevelScene(int num)
    {
        PhotonNetwork.LoadLevel(num);
    }
    public void GotoJumpRopeScence()
    {
        Debug.Log("����");
        pv.RPC(nameof(ChangeLevelScene), RpcTarget.AllBuffered, 2);
    }
    public void GotoCartScence()
    {
        pv.RPC(nameof(ChangeLevelScene), RpcTarget.AllBuffered, 3);        
    }
    public void GotoTopBladeScence()
    {
        pv.RPC(nameof(ChangeLevelScene), RpcTarget.AllBuffered, 4);      
    }
    //��� �г��� ���� �κ�
    private void GetNickName()//�гۻ���
    {
        nickName = nickNameText.text;
        Debug.Log(nickName);
    }
    public override void OnConnectedToMaster()//�����Ͱ� ����Ǹ� �游���
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }
    public override void OnJoinedRoom()//�濡 �������� ��� �÷��̾��� �̸��� ����
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.PlayerList[(int)PublicEnum.Player.Master].NickName = nickName;
        }
        else
        {
            PhotonNetwork.PlayerList[(int)PublicEnum.Player.Client].NickName = nickName;
        }   
    }
    //���Ӹ��� ������ �÷��̾ ������ �����̴�.
    public GameObject MakePlayer(string name, Vector3 playerPos)
    {
        GameObject obj = PhotonNetwork.Instantiate(name, playerPos, Quaternion.identity);
        return obj;
    }
    public int ReturnNumForPhotonViewIsMine()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PhotonView>().IsMine == true)
            {
                return i;
            }
        }
        return 0;
    }
    /*
    private void GetGamePlayer()
    {
        StartCoroutine(CorGetPlayers());
    }
    //������ �÷��̾���� ������
    //��ȯ�� ��� ��ȯ�� �ƴ϶� �����̰� �־ �ڷ�ƾ���� ã�������� �ݺ�
    private IEnumerator CorGetPlayers()
    {
        while (true)
        {
           yield return new WaitForSeconds(0.3f);
            players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 2&& players[players.Length-1] != null)
            {
                Debug.Log("���� ã��");
                break;
            }
            else
            {
                Debug.Log("���� ã�� ���߽��ϴ�");
            }
            yield return new WaitForSeconds(0.3f);
        }
    }*/
}
//���� �÷��̾ �����ϴ� ���� ���� ����Ϸ��ٰ� �ڵ� ��ĥ �� ���Ƽ� ����� ����
public class PlayerManagerClass
{
    public string nickName = "";
    public GameObject thisScenePlayerObject = null;
    public string thisStage = "";
    public PlayerManagerClass(string nickName = "", GameObject thisScenePlayerObject = null, string thisStage = "")
    {
        this.nickName = nickName;
        this.thisScenePlayerObject = thisScenePlayerObject;
        this.thisStage = thisStage;
    }
}