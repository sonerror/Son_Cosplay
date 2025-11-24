using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameUnit : MonoBehaviour
{
  private Transform tf;
  public Transform Tf => tf ? tf : tf = transform;
}