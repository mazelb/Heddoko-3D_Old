#if UNITY_EDITOR
/** 
* @file ContainerTest.cs
* @brief Contains the ContainerTest class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Layout, and layoutcontainer testing
    /// </summary>
    public class ContainerTest : AbstractView
    {
        public LayoutType LayoutType;
        public Layout CurrentLayout;
        void Awake()
        {
            TestCreateLayout();
        }

        void TestCreateLayout()
        {
            if (CurrentLayout == null)
            {
                CurrentLayout = new Layout(LayoutType, this);
            }
        }

    }

}
#endif