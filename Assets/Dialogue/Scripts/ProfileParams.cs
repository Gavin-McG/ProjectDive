using System;
using UnityEngine;
using WolverineSoft.DialogueSystem;

[Serializable]
public class ProfileParams : TextParameters
{
    [SerializeField] public string name;
    [SerializeField] public Sprite profile;
}
