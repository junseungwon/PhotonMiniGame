using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardGameScenceSetting : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button[] goSceneButtons = new Button[3];
    // Start is called before the first frame update
    void Start()
    {
        SettingButtons();
    }
    //������Ӿ��� ��ư �ʹ� ����
    private void SettingButtons()
    {
        goSceneButtons[0].onClick.AddListener(PhotonSystemManager.Instance.GotoJumpRopeScence);
        goSceneButtons[1].onClick.AddListener(PhotonSystemManager.Instance.GotoCartScence);
        goSceneButtons[2].onClick.AddListener(PhotonSystemManager.Instance.GotoTopBladeScence);
    }
}
