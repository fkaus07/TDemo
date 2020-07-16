using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Playerrrrrr : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;


    
    // Start is called before the first frame update
    void Start()
    {

        //myCharacter = PhotonNetwork.Instantiate("Player_SwordMan", Vector3.zero, Quaternion.identity);

        myCharacter = PhotonNetwork.Instantiate(Path.Combine("photonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);

        //myCharacter = PhotonNetwork.Instantiate("PhotonPlayer", Vector3.zero, Quaternion.identity);

        if (myCharacter == null)
        {
            myCharacter = Instantiate(Resources.Load<GameObject>(Path.Combine("photonPrefabs", "PhotonPlayer")), transform.position, transform.rotation, transform);
        }

    }

 

    public void Init()
    {
        if (PV == null)
        {
            PV = GetComponent<PhotonView>();

            if (PV == null)
                return;

            if (PV.IsMine)
            {
                PV.RPC("RPC_addCharacter", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void RPC_AddCharcter()
    {
        //Instantiate(Resources.Load<Sprite>("Ui/SpellIcon/SkillButtonIcon/" + MagicStringList.List[_skillId]));  
        myCharacter = Instantiate(Resources.Load<GameObject>("Player_SwordMan"), transform.position, transform.rotation, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
