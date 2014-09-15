using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;
using Contracts;
using Scrapyard;

namespace KSP4X
{
    public class KSP4X
    {

        public void Start()
        {
            // Missions
            GameEvents.Contract.onAccepted.Add(onContractAccepted);
            GameEvents.Contract.onCancelled.Add(onContractCancelled);
        }

        void onContractAccepted(Contract c)
        {
            if (c.GetType() == typeof(Contracts.Templates.PartTest))
            {
                Contracts.Templates.PartTest test = c as Contracts.Templates.PartTest;
                Debug.Log("You accepted to test " + test.PartName + " " + test.PartIsExperimental);

                if (test.PartIsExperimental)
                {
                    AvailablePart p = PartLoader.LoadedPartsList.Find(x => x.title == test.PartName);
                    Scrapyard.Scrapyard.Instance.Parts.Add(p.name, 1);
                    PopupDialog.SpawnPopupDialog("Experimental part added", "You accepted to test " + test.PartName + ". This is an experimental part, so we will give you one.\n If you succeed you can keep it as reward, but if you loose it and cancel the mission you will have to pay it back!", "ok", false, HighLogic.Skin);
                }
            }
        }
        void onContractCancelled(Contract c)
        {
            if (c.GetType() == typeof(Contracts.Templates.PartTest))
            {
                Contracts.Templates.PartTest test = c as Contracts.Templates.PartTest;
                Debug.Log("You cancelled the test of " + test.PartName + " " + test.PartIsExperimental);
                if (test.PartIsExperimental)
                {
                    AvailablePart p = PartLoader.LoadedPartsList.Find(x => x.title == test.PartName);
                    int qty = (int)Scrapyard.Scrapyard.Instance.Parts.Get(p.name);
                    if (qty > 0)
                    {
                        Scrapyard.Scrapyard.Instance.Parts.Add(p.name, -1);
                        PopupDialog.SpawnPopupDialog("Experimental part removed", "You cancelled an experimental contract, 1 " + test.PartName + "the part has been removed from your inventory.", "ok", false, HighLogic.Skin);
                    }
                    else
                    {
                        PopupDialog.SpawnPopupDialog(
                            "Oh no! You broke it!!!", "We give you high tech toys and you loose them?? We cannot find our " + test.PartName +
                            " in your inventory. I guess you will have to pay us its price... " + p.cost, "ok", false, HighLogic.Skin);
                        Funding.Instance.Funds -= p.cost;
                    }
                }
            }
        }
    }
}
