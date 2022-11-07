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
        userControl = GetComponent<UserControl>();

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
                                break;

                            case "Move":
                                break;

                            case "Left":
                                break;

                            case "Heal":
                                break;

                            case "Attack":
                                break;

                            case "Damage":
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
}