using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Utilities;

public class CharacterStart : CharacterControl
{

  public List<SlotAttachmentPair> slotActiveStart = new List<SlotAttachmentPair>();
  public List<SlotAttachmentPair> slotDeactiveStart = new List<SlotAttachmentPair>();


  void Start()
  {
    foreach (SlotAttachmentPair pair in slotActiveStart)
    {
      TurnSlotAttachment(pair.slotName, pair.attachmentName);
    }

    foreach (SlotAttachmentPair pair in slotDeactiveStart)
    {
      // TurnSlotAttachment(pair.slotName, null);
      SetAlphaSlotName(pair.slotName, 0f);
      Debug.Log("Deactive " + pair.slotName);
    }
  }



}
