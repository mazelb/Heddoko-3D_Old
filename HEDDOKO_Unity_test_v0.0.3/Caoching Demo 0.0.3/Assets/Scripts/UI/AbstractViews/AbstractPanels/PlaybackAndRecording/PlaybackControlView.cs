/** 
* @file PlaybackControlView .cs
* @brief Contains the PlaybackControlView abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls
{
    /// <summary>
    /// Provides a view for playback controls
    /// </summary>
  public  class PlaybackControlView : AbstractSubControl
    {

        public Button PlayButton;
        public Button ForwardButton;
        public Button RewindButton;
    }
}
