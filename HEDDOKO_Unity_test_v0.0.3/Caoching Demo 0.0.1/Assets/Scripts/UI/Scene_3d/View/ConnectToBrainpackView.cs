using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    /// <summary>
    /// The The view connect to the brain pack
    /// </summary>
    public class ConnectToBrainpackView : MonoBehaviour, IInGameMenuItem
    {
        public Button CurrentButton
        {
            get
            {
                return gameObject.GetComponent<Button>();
            }
        }

        public void Hide()
        { 
           // gameObject.SetActive(false);
        }

        public void Show()
        { 
            //gameObject.SetActive(true);
        }
    }
}
