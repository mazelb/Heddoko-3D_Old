﻿/** 
* @file AbstractView.cs
* @brief Contains the AbstractView abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews
{

    /// <summary>
    /// An abstract class which provides an interface for views inside the app to inherit from. 
    /// </summary>
    public abstract class AbstractView: MonoBehaviour
    {
        private RectTransform mCurRectTransform;
        /// <summary>
        /// The view the is proceeding the current one
        /// </summary>
        public AbstractView NextView;

        /// <summary>
        /// The view that will be preceeding the current one 
        /// </summary>
        public AbstractView PreviousView;

        private Layout mViewLayout;

        public RectTransform RectTransform
        {
            get
            {
                if (mCurRectTransform == null)
                {
                    mCurRectTransform= gameObject.GetComponent<RectTransform>(); 
                }
                return mCurRectTransform;
            }
           
        }

        public virtual void Hide()
        { }

        public virtual void Show()
        {
            
        }

        public virtual void Back()
        {
            Hide();
            PreviousView.Show();
        }
        /// <summary>
        /// traverses the views backwards until null is reached.
        /// </summary>
        public void ReturnHome()
        {
            AbstractView vCurrentView = this;
            while (PreviousView != null)
            {
                vCurrentView.Hide();
                vCurrentView = vCurrentView.PreviousView;
            }
        }

    }
}
