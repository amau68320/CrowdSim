using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkEvent : MonoBehaviour
{
    public delegate void TalkAction();
    public static event TalkAction onTalk;
}
