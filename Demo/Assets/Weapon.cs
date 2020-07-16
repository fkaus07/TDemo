using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PhotonView PV;
    public Np rootObj;
    // Start is called before the first frame update
    void Start()
    {
        rootObj = transform.root.GetComponent<Np>();
        PV = rootObj.PV;
    }

    // Update is called once per frame
    void OnTriggerStay2D(Collider2D other)
    {
        //if (other.tag == "Player" && !rootObj.isOneDamage && !(other.GetComponent<PhotonView>().IsMine))
        if (other.tag == "Player")// && !rootObj.isOneDamage && !(other.GetComponent<PhotonView>().IsMine))
        {
            if(!PV.IsMine)
            {
                if (rootObj.isAttackRunning == true)
                {
                    Debug.Log("피격중");
                    hitTarget(other);
                }

                if (rootObj.GetComponent<Np>().m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    Debug.Log("공격중");
                else if (rootObj.GetComponent<Np>().m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    Debug.Log("걷는중");
                else if (rootObj.GetComponent<Np>().m_Anim.GetCurrentAnimatorStateInfo(0).IsName("IDLE"))
                    Debug.Log("IDLE중");
            }
            /*
            if (rootObj.isAttacking == true && !rootObj.isOneDamage && !(other.GetComponent<PhotonView>().IsMine))
            {
                rootObj.isOneDamage = true;
                other.GetComponent<Np>().Hit();
                Debug.Log("적공격성공");
            }
            */
        }
    }
    
    void hitTarget(Collider2D other)
    {
        if (rootObj.isOneDamage)
            return; 

            rootObj.isOneDamage = true;
            other.GetComponent<Np>().Hit(true);
            Debug.Log("적공격성공");      
    }

}
