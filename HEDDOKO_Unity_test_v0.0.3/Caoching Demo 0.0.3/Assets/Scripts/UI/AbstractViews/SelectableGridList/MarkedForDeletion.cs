/** 
* @file MarkedForDeletion.cs
* @brief Contains the MarkedForDeletion class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Utils.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    /// <summary>
    ///  Represents an item that is marked for deletion
    /// </summary>
    public class MarkedForDeletion : MonoBehaviour 
    {
        public FontAwesomeSpriteContainer Container;
        public Image MarkedForDeletionIcon;
        public Button Button;
        public Color MarkedForDeletionColor;
        public Color UnmarkedForDeletionColor;
  private bool mMarkForDeletion;

        /// <summary>
        /// Getter and setter. Setter changes the state of 
        /// the object
        /// </summary>
        public bool IsMarkedForDeletion
        {
            get
            {
                return mMarkForDeletion;
            }
            set
            {
                Debug.Log("Set called");
                Color vCurrent = MarkedForDeletionIcon.color;
                mMarkForDeletion = value;
                if (mMarkForDeletion)
                {
                    vCurrent = MarkedForDeletionColor;
                }
                else
                {
                    vCurrent = UnmarkedForDeletionColor;
                }
                MarkedForDeletionIcon.color = vCurrent;
            }
        }
        void Awake()
        {
            MarkedForDeletionIcon.sprite = Container.GetSpriteAt(21); 
            IsMarkedForDeletion = false;
            Button = GetComponent<Button>();
            Button.onClick.AddListener(()=> { IsMarkedForDeletion = !IsMarkedForDeletion; });
        }


   
    }
}
