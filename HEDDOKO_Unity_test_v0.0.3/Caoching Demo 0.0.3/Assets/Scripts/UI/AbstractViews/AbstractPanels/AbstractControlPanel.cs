﻿using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;

namespace Assets.Scripts.UI.AbstractPanels
{
    public class AbstractControlPanel
    {
        private List<AbstractSubControl> mSubControls;
        public void UpdateSubControlList(List<AbstractSubControl> vSubcontrols)
        {
            throw new System.NotImplementedException();
        }
    }
}