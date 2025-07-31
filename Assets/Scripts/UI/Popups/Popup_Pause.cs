using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Pause : PopupBase
{
    private void Start()
    {
        DataManager dm = DataManager.Instance;

        dm.GetProjectileData(1);
    }
}
