using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class NPCTextCreator : ScriptableObject
{
    public List<string> greetings = new List<string>();
    public List<string> quest = new List<string>();
    public List<string> thanks = new List<string>();
}
