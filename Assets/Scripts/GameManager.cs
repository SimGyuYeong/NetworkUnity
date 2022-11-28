using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    const char CHAR_TERMINATOR = ';';
    const char CHAR_COMMA = ',';

    private UserControl userControl;

    [SerializeField]
    private TMP_InputField nickName;
    [SerializeField]
    private TMP_InputField Chat;
    string myID;

    public GameObject User;

    Dictionary<string, UserControl> remoteUsers;
    Queue<string> commandQueue;

    private void Start()
    {
        userControl = User.GetComponent<UserControl>();

        remoteUsers = new Dictionary<string, UserControl>();
        commandQueue = new Queue<string>();
    }

    public void Update()
    {
        
    }

    public void SendCommand(string cmd)
    {
        SocketModule.GetInstance().SendData(cmd);
        Debug.Log("cmd send: " + cmd);
    }

    public void QueueCommand(string cmd)
    {
        commandQueue.Enqueue(cmd);
    }

    public void ProcessQueue()
    {
        while(commandQueue.Count>0)
        {
            string nextCommand = commandQueue.Dequeue();
            ProcessCommand(nextCommand);
        }
    }

    public void OnLogin()
    {
        myID = nickName.text;
        if(myID.Length>0)
        {
            SocketModule.GetInstance().Login(myID);
            User.transform.position = Vector3.zero;
        }
    }

    public void OnLogOut()
    {
        SocketModule.GetInstance().Logout();
        foreach(var user in remoteUsers)
        {
            Destroy(user.Value.gameObject);
        }
        remoteUsers.Clear();
    }

    public void OnRevive()
    {
        userControl.Revive();
        string data = "#Heal#";
        SendCommand(data);
    }

    public void OnMessage()
    {
        if(myID!=null)
        {
            SocketModule.GetInstance().SendData(Chat.text);
        }
    }

    public void AddUser(string id)
    {
        if (!remoteUsers.ContainsKey(id))
        {
            UserControl userControl = new GameObject().AddComponent<UserControl>();

            remoteUsers.Add(id, userControl);
        }

    }

    public void UserLeft(string id)
    {
        if (remoteUsers.ContainsKey(id))
        {
            Destroy(remoteUsers[id].gameObject);
            remoteUsers.Remove(id);
        }
    }

    public void UserHeal(string id)
    {
        if(remoteUsers.ContainsKey(id))
        {
            remoteUsers[id].Revive();
        }
    }

    public void TakeDamage(string remain)
    {
        var strs = remain.Split(CHAR_COMMA);
        for(int i=0;i<strs.Length;i++)
        {
            if(remoteUsers.ContainsKey(strs[i]))
            {
                UserControl uc = remoteUsers[strs[i]];
                if(uc!=null)
                {
                    uc.DropHp(10);
                }
            }
            else
            {
                if(myID.CompareTo(strs[i])==0)
                {
                    userControl.DropHp(10);
                }
            }
        }
    }

    public void SetMove(string id, string cmdMove)
    {
        if (remoteUsers.ContainsKey(id))
        {
            UserControl uc = remoteUsers[id];
            string[] strs = cmdMove.Split(CHAR_COMMA);
            Vector3 pos = new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), 0);
            uc.transform.position = pos;
        }
    }

    public void ProcessCommand(string cmd)
    {
        bool isMore = true;
        while(isMore)
        {
            Debug.Log("Process cmd = " + cmd);
            int nameidx = cmd.IndexOf("$");
            string id = "";
            if(nameidx>0)
            {
                id = cmd.Substring(0, nameidx);
            }
            int cmdidx1 = cmd.IndexOf("#");
            if(cmdidx1>nameidx)
            {
                int cmdidx2 = cmd.IndexOf("#", cmdidx1 + 1);
                if(cmdidx2>cmdidx1)
                {
                    string command = cmd.Substring(cmdidx1 + 1, cmdidx2 - cmdidx1 - 1);
                    string remain = "";
                    string nextCommand;
                    int endidx = cmd.IndexOf(CHAR_TERMINATOR, cmdidx2 + 1);
                    if(endidx>cmdidx2)
                    {
                        remain = cmd.Substring(cmdidx2 + 1, endidx - cmdidx2 - 1);
                        nextCommand = cmd.Substring(endidx + 1);
                    }
                    else
                    {
                        nextCommand = cmd.Substring(cmdidx2 + 1);
                    }
                    Debug.Log("command = " + command + "id = " + id + "remain = " + remain + "next = " + nextCommand);

                    if(myID.CompareTo(id)!=0)
                    {
                        switch(command)
                        {
                            case "Enter":
                                AddUser(id);
                                break;
                            case "Move":
                                SetMove(id, remain);
                                break;
                            case "Left":
                                UserLeft(id);
                                break;
                            case "Heal":
                                UserHeal(id);
                                break;
                            case "Damage":
                                TakeDamage(remain);
                                break;
                        }
                    }
                    else 
                    {
                        Debug.Log("Skip");
                    }
                }
                else
                {
                    isMore = false;
                }
            }
            else
            {
                isMore = false;
            }
        }
    }
}