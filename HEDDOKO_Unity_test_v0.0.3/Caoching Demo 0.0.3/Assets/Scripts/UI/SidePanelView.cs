﻿
/** 
* @file SidePanelView.cs
* @brief Contains the SidePanelView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews;
using UIWidgets;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The side panel in the app main screen
    /// </summary>
    public class SidePanelView : AbstractView
    {
        public TreeView Tree;
        private ObservableList<TreeNode<TreeViewItem>> mNodes;
        public override void CreateDefaultLayout()
        {

            var config = new List<int>() { 6,2 };
            var mNameConfig = new List<string>()
            {
                "DASHBOARD","MOVEMENT","MOVEMENT TESTS","MY ACCOUNT","SETTINGS","HELP"
            };
            mNodes = GenerateTreeNodes(config, vIsExpanded: true);
            for(int i = 0 ; i < mNodes.Count; i ++)
            {
                mNodes[i].Item.Name = mNameConfig[i];
            }
            Tree.Start();
            Tree.Nodes = mNodes;
            //var vTest_Item = new TreeViewItem("added");
           // var vTest_node = new TreeNode<TreeViewItem>(vTest_Item);
            //mNodes.Add(vTest_node);


           // mNodes[1].IsVisible = false;
          //  mNodes[2].Nodes[1].IsVisible = false;

        }

        void Awake()
        {
            CreateDefaultLayout();
        }
        private ObservableList<TreeNode<TreeViewItem>> GenerateTreeNodes(List<int> vItems, bool vIsExpanded )
        {

            return Enumerable.Range(1, vItems[0]).Select(x =>
             {
                 
                 var vItem = new TreeViewItem("test");
                 var vNodes = vItems.Count > 1
                     ? GenerateTreeNodes(vItems.GetRange(1, vItems.Count - 1), vIsExpanded )
                     : null;
                 return new TreeNode<TreeViewItem>(vItem, vNodes, vIsExpanded);
             }).ToObservableList();
        }
    }
}