using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Np : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum AniType {Idle, Attack, Run, Sit, Jump, Die, Max}
    string[] AniActionList = new string[] { "Idle", "Attack", "Run", "Sit", "Jump", "Die" };
    public bool IsSit = false;
    public int currentJumpCount = 0;
    public bool isGrounded = false;
    public bool OnceJumpRayCheck = false;

    public bool Is_DownJump_GroundCheck = false;   // 다운 점프를 하는데 아래 블록인지 그라운드인지 알려주는 불값
    protected float m_MoveX;
    public Rigidbody2D m_rigidbody;
    protected CapsuleCollider2D m_CapsulleCollider;    

    [Header("[Setting]")]
    public float MoveSpeed = 6;
    public int JumpCount = 2;
    public float jumpForce = 15f;

    public PhotonView PV;
    public Animator m_Anim;
    public AniType curAniType = AniType.Max;

    float attackDelay = 0.5f;
    public bool isAttacking = false;
    public bool isAttackRunning = false;
    public bool isRun = false;
    public GameObject HpObj;
    public Image hpImage;
    public bool isOneDamage = false;
    public Vector3 curPos = new Vector3();

    const float maxlatencyDistance = 5.0f;
    const float maxlatencyDistanceY = 2.0f;

    private void Start()
    {
        m_CapsulleCollider = this.transform.GetComponent<CapsuleCollider2D>();        
        m_rigidbody = this.transform.GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        m_Anim = this.transform.GetComponent<Animator>();
        m_Anim.applyRootMotion = true;
        hpImage = HpObj.GetComponent<Image>();
    }

    private void Update()
    {
        if (PV.IsMine)
            MoveCheck();
        else
        {
            m_Anim.applyRootMotion = true;
            FindState();
            PlayAni(AniType.Idle);

            float disTance = (transform.position - curPos).sqrMagnitude;
            if (disTance >= maxlatencyDistance)
                transform.position = curPos;
            else if (Mathf.Abs(transform.position.y - curPos.y) > maxlatencyDistanceY)
            {
                curPos.x = transform.position.x;
                transform.position = curPos;
            }          
        }    
    }

    public void Hit(bool isLeft)
    {
       hpImage.fillAmount -= 0.2f;              
       PV.RPC("HitRpc", RpcTarget.AllBuffered, true);
    }

    [PunRPC]
    public void HitRpc(bool isLeft)
    {        
        m_rigidbody.AddForce(Vector2.up * jumpForce / 3.0f, ForceMode2D.Impulse);

        OnceJumpRayCheck = true;
        isGrounded = false;

        currentJumpCount++;
    }
   
    public void FindState()
    {
        
        if(m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (!isAttackRunning)
            {
                m_Anim.SetBool("Attack", false);
                Debug.Log("강제해제");
            }
        }

        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isAttacking = false;
            isAttackRunning = false;
            isRun = false;
        }

        if ((isRun || isAttackRunning))
            return;

        for (int idx = 0; idx < (int)AniType.Max; idx++)
        {
            if (m_Anim.GetBool(AniActionList[idx]))
            {
                if (idx == (int)AniType.Attack)
                {
                    isAttacking = true;                    
                    isRun = false;
                    StartCoroutine("AttackAniCheck");
                }
                else if (idx == (int)AniType.Run)
                {                   
                    isRun = true;
                }
                else if (idx == (int)AniType.Idle)
                {
                    isAttacking = false;
                    isAttackRunning = false;
                    isRun = false;
                }
            }
            else
            {
                if (idx == (int)AniType.Attack)
                {
                    isAttacking = false;
                    isAttackRunning = false;
                }
                else if (idx == (int)AniType.Run)
                {                   
                    isRun = false;
                }

                m_Anim.SetBool(AniActionList[idx], false);
            }
        }

    }


    public void MoveCheck()
    {
        checkInput();

        if (m_rigidbody.velocity.magnitude > 30)
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x - 0.1f, m_rigidbody.velocity.y - 0.1f);
        }
    }

    [PunRPC]
    void FlipXRPC(bool bLeft)
    {
        transform.localScale = new Vector3(bLeft ? 1 : -1, 1, 1);
        hpImage.fillOrigin = bLeft ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
    }

    [PunRPC]
    void prefromJump()
    {       
        m_rigidbody.velocity = new Vector2(0, 0);

        m_rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        OnceJumpRayCheck = true;
        isGrounded = false;


        currentJumpCount++;
    }

    [PunRPC]
    void DownJump()
    {
        if (!isGrounded)
            return;


        if (!Is_DownJump_GroundCheck)
        {
            //PlayAni(AniType.Jump);

            m_rigidbody.AddForce(-Vector2.up * 10);
            isGrounded = false;

            m_CapsulleCollider.enabled = false;

            StartCoroutine(GroundCapsulleColliderTimmerFuc());

        }
    }

    void PlayAni(AniType action)
    {
    
        if ((isRun ||  isAttackRunning) && (int)AniType.Idle == action)
        {
            m_Anim.SetBool("Idle", false);
            return;
        }
     
        for (int idx = 0; idx < (int)AniType.Max; idx++)
        {
            
            if (idx == (int)AniType.Attack && idx == (int)action)
            {
                if (!isAttacking && PV.IsMine)
                {
                    StartCoroutine("AttackAniCheck");
                    //m_Anim.SetBool(AniActionList[idx], false);
                    //m_Anim.SetBool("Idle", true);
                    continue;
                }
                else
                    continue;

            }         
            else
            if (idx == (int)action)
            {               
                    m_Anim.SetBool(AniActionList[idx], true);
            }
            else
            {
                    m_Anim.SetBool(AniActionList[idx], false);
            }
        }

        curAniType = action;

    }

    void StopAni(AniType action)
    {
        m_Anim.SetBool(AniActionList[(int)action], false);
    }

    IEnumerator AttackAniCheck()
    {
        Debug.Log("in AttackAniCheck");   
        float curTime = 0.0f;
        m_Anim.SetBool("Attack", true);
        isAttackRunning = true;

        while (true)
        {
            curTime += Time.deltaTime;

            if (curTime > attackDelay)
            {
                isAttacking = false;
                isAttackRunning = false;
                isOneDamage = false;

                if (PV.IsMine)
                {
                    m_Anim.SetBool("Attack", false);
                    m_Anim.SetBool("Idle", true);
                }
                else
                {
                    m_Anim.SetBool("Idle", true);
                    Debug.Log("클론 공격 해제");
                }
                yield break;
            }
            else
                yield return null; //무한루프 방지        
        }
    }





    public void checkInput()
    {

        if (Input.GetKeyDown(KeyCode.S))  //아래 버튼 눌렀을때. 
        {
            IsSit = true;
            PlayAni(AniType.Sit);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            //m_Anim.SetBool("Idle", true);
            PlayAni(AniType.Idle);
            IsSit = false;
        }


        // sit나 die일때 애니메이션이 돌때는 다른 애니메이션이 되지 않게 한다. 
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Sit") || m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentJumpCount < JumpCount)  // 0 , 1
                {
                    PV.RPC("DownJump", RpcTarget.AllBuffered);
                    //DownJump();
                }
            }

            return;
        }


        m_MoveX = Input.GetAxis("Horizontal");

        GroundCheckUpdate();


        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {    
                PlayAni(AniType.Attack);
            }
            else
            {

                if (m_MoveX == 0)
                {
                    if (!OnceJumpRayCheck)
                        PlayAni(AniType.Idle);           
                }
                else
                {
                }

            }
        }


        if (Input.GetKey(KeyCode.Alpha1))
        {
            PlayAni(AniType.Die);

        }

        // 기타 이동 인풋.
        isRun = false;

        if (Input.GetKeyUp(KeyCode.A))
        {
            StopAni(AniType.Run);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StopAni(AniType.Run);
        }

        if (Input.GetKey(KeyCode.D))
        {
            isRun = true;
            if (isGrounded)  // 땅바닥에 있었을때. 
            {



                if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    return;

                transform.transform.Translate(Vector2.right* m_MoveX * MoveSpeed * Time.deltaTime);
                PlayAni(AniType.Run);
            }
            else
            {
                transform.transform.Translate(new Vector3(m_MoveX * MoveSpeed * Time.deltaTime, 0, 0));
                PlayAni(AniType.Run);
            }




            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            if (!Input.GetKey(KeyCode.A))
                Filp(false);


        }
        else if (Input.GetKey(KeyCode.A))
        {

            isRun = true;
            if (isGrounded)  // 땅바닥에 있었을때. 
            {
                if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    return;

                transform.transform.Translate(Vector2.right * m_MoveX * MoveSpeed * Time.deltaTime);
                PlayAni(AniType.Run);
            }
            else
            {
                transform.transform.Translate(new Vector3(m_MoveX * MoveSpeed * Time.deltaTime, 0, 0));
                PlayAni(AniType.Run);
            }


            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;

            if (!Input.GetKey(KeyCode.D))
                Filp(true);


        }

       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;


            if (currentJumpCount < JumpCount)  // 0 , 1
            {

                if (!IsSit)
                {
                    PV.RPC("prefromJump", RpcTarget.AllBuffered);
   

                }
                else
                {
                    PV.RPC("DownJump", RpcTarget.AllBuffered);
  
                }

            }


        }



    }





    protected void LandingEvent()
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Run") && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            m_Anim.SetBool("Idle", true);
    }


    protected void Filp(bool bLeft)
    {
        PV.RPC("FlipXRPC", RpcTarget.AllBuffered, bLeft);
    }

    IEnumerator GroundCapsulleColliderTimmerFuc()
    {
        yield return new WaitForSeconds(0.3f);
        m_CapsulleCollider.enabled = true;
    }


    //////바닥 체크 레이케스트 
    Vector2 RayDir = Vector2.down;


    float PretmpY;
    float GroundCheckUpdateTic = 0;
    float GroundCheckUpdateTime = 0.01f;
    protected void GroundCheckUpdate()
    {
        if (!OnceJumpRayCheck)
            return;

        GroundCheckUpdateTic += Time.deltaTime;



        if (GroundCheckUpdateTic > GroundCheckUpdateTime)
        {
            GroundCheckUpdateTic = 0;



            if (PretmpY == 0)
            {
                PretmpY = transform.position.y;
                return;
            }



            float reY = transform.position.y - PretmpY;  //    -1  - 0 = -1 ,  -2 -   -1 = -3

            if (reY <= 0)
            {

                if (isGrounded)
                {

                    LandingEvent();
                    OnceJumpRayCheck = false;

                }
                else
                {

                    Debug.Log("안부딪힘");

                }


            }


            PretmpY = transform.position.y;

        }




    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);            
            stream.SendNext(hpImage.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            hpImage.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
