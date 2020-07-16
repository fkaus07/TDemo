using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null );

    public override void OnJoinedRoom(){
        PhotonNetwork.Instantiate("Np", Vector3.zero, Quaternion.identity);
    }

    /*
       public override void OnConnectedToMaster() => Ph
       {
           Debug.Log("접속중 " + PhotonNetwork.CloudRegion +   "server!");
           PhotonNetwork.AutomaticallySyncScene = true;
           PhotonNetwork.JoinRandomRoom();       
       }
       */

    /*
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("조인룸 실패 ");
        CreateRoom();

        player.GetComponent<Player>().Init();
    }

    void CreateRoom()
    {
        Debug.Log("방생성 시도");
        PhotonNetwork.CreateRoom("Room1");

        player.GetComponent<Player>().Init();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("룸생성 실패 ");
        CreateRoom();

        player.GetComponent<Player>().Init();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방생성 성공");
        player.GetComponent<Player>().Init();
    }

    */

}
