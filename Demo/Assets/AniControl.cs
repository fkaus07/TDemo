using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControl : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    protected Animator m_Anim;
    // Start is called before the first frame update
    void Start()
    {
        m_Anim = this.transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set()
    {

    }

    public void Idle()
    {

    }

    public void Run()
    {

    }

    public void Attack()
    {
        //m_Anim.Play("Attack");
        m_Anim.SetBool("Attack", true);        
    }
}
