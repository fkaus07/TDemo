using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public GameObject charecterObj;
    public PhotonView PV;
    private Swordman targetObj;
    bool firstInit = false;

    // Start is called before the first frame update
    void Start()
    {
        //charecterObj = PhotonNetwork.Instantiate("Charecter", Vector3.zero, Quaternion.identity);
        //charecterObj = Instantiate<GameObject>("Charecter", Vector3.zero, Quaternion.identity);
        charecterObj= Instantiate(Resources.Load<GameObject>("Charecter"), transform.position, transform.rotation, transform);
        targetObj = charecterObj.GetComponent<Swordman>();
        //PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstInit)
        {
            firstInit = true;
            return;
        }

        if (PV.IsMine)
        {
            targetObj.MoveCheck();

         //   transform.position += targetObj.transform.position;
          //targetObj.transform.position = new Vector3(0, 0, 0);

          //  transform.rotation = targetObj.transform.rotation;
          //  targetObj.transform.rotation = Quaternion.identity;
        }
    }
}
