/** 
* @file Layout.cs
* @brief Contains the Layout   class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using Assets.Scripts.UI.Analysis;
using Assets.Scripts.UI.Layouts;

namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    /// <summary>
    /// Acts as an identifier for a LayoutContainer
    /// </summary>
    public class Layout
    {
        private Guid mId = new Guid();
        private LayoutContainer mLayoutContainer;

        public Layout(LayoutType vLayoutType)
        {
            
        }
    }
}
