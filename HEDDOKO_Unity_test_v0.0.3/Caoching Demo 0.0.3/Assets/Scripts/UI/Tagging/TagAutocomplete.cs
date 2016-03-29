/** 
* @file TagAutocomplete.cs
* @brief Contains the TagAutocomplete class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// An input field that contains a listview, displaying results of inserted text in the
    /// input field
    /// </summary>
    [Serializable]
    public class TagAutocomplete : MonoBehaviour, IPointerClickHandler, IDeselectHandler
    {
        private InputField mInsertTagInputField;

        public int MaxResults;
        public int MinStringLength;
        public ListView FloatingListView;
        private RectTransform mRectTransform;
        private List<Tag> mPreviousSearchResults = new List<Tag>();
        private EventSystem mEventSystem;
        /// <summary>
        /// The localized input field
        /// </summary>
        public InputField InsertTagInputField
        {
            get
            {
                if (mInsertTagInputField == null)
                {
                    mInsertTagInputField = GetComponent<InputField>();
                }
                return mInsertTagInputField;
            }
        }
        public TaggingManager TaggingManager { get; set; }
        public TaggingContainer TagContainer { get; set; }

        public RectTransform RectTransform
        {
            get
            {
                if (mRectTransform == null)
                {
                    mRectTransform = GetComponent<RectTransform>();
                }
                return mRectTransform;
            }
            set { mRectTransform = value; }
        }

        public EventSystem EventSystem
        {
            get
            {
                if (mEventSystem == null)
                {
                    mEventSystem = FindObjectOfType<EventSystem>();
                }
                return mEventSystem;
            }
            set { mEventSystem = value; }
        }

        void Awake()
        {
            InsertTagInputField.onValueChange.AddListener(Search); 
        }
 

        public void Show()
        {
            FloatingListView.OnSelectString.AddListener(SelectItem);
            FloatingListView.OnDeselect.AddListener(DeselectedList);
        }

       
        /// <summary>
        /// Hides the input field from view
        /// </summary>
        public void Hide()
        {
            InsertTagInputField.text = string.Empty;
            FloatingListView.gameObject.SetActive(false);
            FloatingListView.OnSelectString.RemoveListener(SelectItem);
            FloatingListView.OnDeselect.RemoveListener(DeselectedList);

        }

        /// <summary>
        /// Search for tags
        /// </summary>
        /// <param name="vSearchString"></param>
        public void Search(string vSearchString)
        {
            ShowList();
            if (vSearchString.Length < MinStringLength)
            {
                ClearList();

            }
            else
            {
                List<Tag> vResults = TaggingManager.FindTagByPartialTitle(vSearchString, MaxResults);
                mPreviousSearchResults = vResults;
                FloatingListView.DataSource.BeginUpdate();
                FloatingListView.Clear();
                for (int i = 0; i < vResults.Count; i++)
                {
                    FloatingListView.DataSource.Add(mPreviousSearchResults[i].Title);
                }
                FloatingListView.DataSource.EndUpdate();

            }
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        private void ClearList()
        { 
            FloatingListView.Clear();
            mPreviousSearchResults.Clear(); 
        }

        private void ShowList()
        {
            if (!FloatingListView.gameObject.activeSelf)
            {
                FloatingListView.gameObject.SetActive(true);
            }
            FloatingListView.gameObject.SetActive(true);
            float vSizeX = RectTransform.sizeDelta.x;
            Vector2 vFloatingListSize = FloatingListView.RectTransform.sizeDelta;
            vFloatingListSize.x = vSizeX;
            FloatingListView.RectTransform.sizeDelta = vFloatingListSize;
            Vector3[] vWorldCorners = new Vector3[4];
            RectTransform.GetWorldCorners(vWorldCorners);
            //set the position in the midpoint between the bottom two corners
            Vector2 vNewFloatingListPos = vWorldCorners[3] - vWorldCorners[0];
            vNewFloatingListPos /= 2f;
            vNewFloatingListPos.x = vWorldCorners[0].x;
            vNewFloatingListPos.y = vWorldCorners[0].y;
            FloatingListView.RectTransform.position = vNewFloatingListPos;
        }

        public void OnPointerClick(PointerEventData eventData)
        { 
             
        }

        public void OnDeselect(BaseEventData eventData)
        {
            //Check if mouse is above the list view rect
            Vector2 vMousePos = Input.mousePosition; 
            if (!RectTransformUtility.RectangleContainsScreenPoint(FloatingListView.RectTransform, vMousePos,
                   Camera.main))
            {
                Hide();
            } 

        }
        /// <summary>
        /// When an item has been selected
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        public void SelectItem(int index, string text)
        {
            Debug.Log("Item selected " + text);
        }

        /// <summary>
        /// List has been deselected
        /// </summary>
        /// <param name="vItemnum"></param>
        /// <param name="vItem"></param>
        private void DeselectedList(int vItemnum, ListViewItem vItem)
        {
            Debug.Log("Deselected list");
        }
    }
}