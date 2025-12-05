using System.Collections.Generic;
using HoangHH;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{

  public ShowObjectEffect Gr1, Gr2;

  public List<Item> items = new List<Item>();

  public void ChangeGroupItemStart()
  {
    Debug.Log("ChangeGroupItemStart ");
    Gr1.Hide(0.5f);
    Gr2.Show(1f);
  }
}
