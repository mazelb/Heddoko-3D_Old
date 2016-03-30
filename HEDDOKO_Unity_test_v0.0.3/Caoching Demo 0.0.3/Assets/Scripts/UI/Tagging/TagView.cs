/** 
* @file TagView.cs
* @brief Contains the TagView class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tagging
{
    public class TagView : MonoBehaviour
    {
        public Button RemoveTagButton;
        public TaggingContainer TaggingContainer;
        public Tag Tag;
        public Text TagText;
    }
}