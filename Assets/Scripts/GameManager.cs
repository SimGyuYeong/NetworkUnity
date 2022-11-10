using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    const char CHAR_TERMINATOR = ';';
    const char CHAR_COMMA = ',';

    private UserControl userControl;

    [SerializeField]
    private InputField nickName;
    [SerializeField]
    private InputField Chat;
    string myID;

    public GameObject prefabUser;
    public GameObject User;

    Dictionary<string, UserControl> remoteUsers;
    Queue<string> commandQueue;

    private void Start()
    {
        userControl = FindObjectOfType<UserControl>();

        remoteUsers = new Dictionary<string, UserControl>();
        commandQueue = new Queue<string>();
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SendCommand("#Attack#");
            }
        }
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

    private void ProcessQueue()
    {
        while(commandQueue.Count > 0)
        {
            string nextCommand = commandQueue.Dequeue();
            ProcessCommand(nextCommand);    
        }
    }

    public void ProcessCommand(string cmd)
    {
        bool isMore = true;
        while(isMore)
        {
            Debug.Log("Process cmd = " + cmd);
            
            //ID
            int nameIdx = cmd.IndexOf("$");
            string id = "";
            if(nameIdx > 0)
            {
                id = cmd.Substring(0, nameIdx);
            }

            // Command
            int cmdIdx1 = cmd.IndexOf("#");
            if(cmdIdx1 > nameIdx)
            {
                int cmdIdx2 = cmd.IndexOf("#", cmdIdx1 + 1);
                if(cmdIdx2 > cmdIdx1)
                {
                    string command = cmd.Substring(cmdIdx1 + 1, cmdIdx2 - cmdIdx1 - 1);

                    // End
                    string remain = "";
                    string nextCommand;
                    int endIdx = cmd.IndexOf(CHAR_TERMINATOR, cmdIdx2 + 1);
                    if(endIdx > cmdIdx2)
                    {
                        remain = cmd.Substring(cmdIdx2 + 1, endIdx - cmdIdx2 - 1);
                        nextCommand = cmd.Substring(endIdx + 1);
                    }
                    else
                    {
                        nextCommand = cmd.Substring(cmdIdx2 + 1);
                    }
                    Debug.Log("command = " + command + " ID = " + id + " Remain = " + remain + " Next = " + nextCommand);
                
                    if(myID.CompareTo(id) != 0)
                    {
                        switch (command)
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

                            default:
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

    public void OnLogin()
    {
        myID = nickName.text;
        if(myID.Length > 0)
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
        SendCommand("#Heal#");
        
    }

    public void OnMessage()
    {
        SocketModule.GetInstance().SendData(Chat.text);
    }

    public void AddUser(string id)
    {
        UserControl uc = null;
        if(!remoteUsers.ContainsKey(id))
        {
            GameObject newUser = Instantiate(prefabUser);
            uc = newUser.GetComponent<UserControl>();
            remoteUsers.Add(id, uc);
        }
    }

    public void UserLeft(string id)
    {
        if(remoteUsers.ContainsKey(id))
        {
            UserControl uc = remoteUsers[id];
            Destroy(uc.gameObject);
            remoteUsers.Remove(id);
        }
    }

    public void UserHeal(string id)
    {
        if(remoteUsers.ContainsKey(id))
        {
            UserControl uc = remoteUsers[id];
            uc.Revive();
        }
    }

    public void TakeDamage(string remain)
    {
        var strs = remain.Split(',');
        for(int i = 0; i < strs.Length; i++)
        {
            if (remoteUsers.ContainsKey(strs[i]))
            {
                UserControl uc = remoteUsers[strs[i]];
                if(uc != null)
                {
                    uc.DropHp(10);
                }
            }
        }
    }

    public void SetMove(string id, string cmdMove)
    {
        if(remoteUsers.ContainsKey(id))
        {
            UserControl uc = remoteUsers[id];
            string[] strs = cmdMove.Split(',');
            Vector3 pos = new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), 0);
            uc.targetPos = pos;
        }
    }
}