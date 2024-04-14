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

    private void Awake()//처음 세팅
    {
        pv = GetComponent<PhotonView>();
        DontDestroyOnLoad(this);
        Screen.SetResolution(1200, 1000, false);
    }
    //시작화면에서 게임을 진행할 보드게임씬으로 이동한다.
    //1ㄷ1로 해서 방은 들어가자 마자 생성이 된다.
    public void GoToBoardGameScence()
    {
        GetNickName();
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LoadLevel(1);       
    }
    //줄넘기 , 카트 , 팽이 게임 씬으로 이동하기 버튼용
    [PunRPC]
    private void ChangeLevelScene(int num)
    {
        PhotonNetwork.LoadLevel(num);
    }
    public void GotoJumpRopeScence()
    {
        Debug.Log("눌림");
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
    //잠깐 닉넴을 받은 부분
    private void GetNickName()//닉넴생성
    {
        nickName = nickNameText.text;
        Debug.Log(nickName);
    }
    public override void OnConnectedToMaster()//마스터가 연결되면 방만들기
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }
    public override void OnJoinedRoom()//방에 참가했을 경우 플레이어의 이름을 저장
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
    //게임마다 생성될 플레이어를 생성할 공간이다.
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
    //스폰된 플레이어들을 가져옴
    //소환후 즉시 소환이 아니라 딜레이가 있어서 코루틴으로 찾을때까지 반복
    private IEnumerator CorGetPlayers()
    {
        while (true)
        {
           yield return new WaitForSeconds(0.3f);
            players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 2&& players[players.Length-1] != null)
            {
                Debug.Log("전부 찾음");
                break;
            }
            else
            {
                Debug.Log("전부 찾지 못했습니다");
            }
            yield return new WaitForSeconds(0.3f);
        }
    }*/
}
//원래 플레이어를 관리하는 툴로 만들어서 사용하려다가 코드 겹칠 것 같아서 사용은 안함
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