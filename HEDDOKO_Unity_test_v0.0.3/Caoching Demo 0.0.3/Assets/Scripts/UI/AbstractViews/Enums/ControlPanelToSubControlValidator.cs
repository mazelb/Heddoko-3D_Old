/** 
* @file ControlPanelToSubControlValidator.cs
* @brief Contains the ControlPanelToSubControlValidator  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Windows.Forms;

namespace Assets.Scripts.UI.AbstractViews.Enums
{
    /// <summary>
    /// Validates the correct mapping Control panels to SubControls
    ///Example: SuitConnectionSubControls should not be panelled by a DebugControlPanel
    /// </summary>
    public static class ControlPanelToSubControlValidator
    {
        /// <summary>
        /// Validates a control panel to subcontrols
        /// </summary>
        /// <param name="vCtrlPanelType"></param>
        /// <param name="vSubCtrlType"></param>
        /// <returns></returns>
        public static bool Validate(ControlPanelType vCtrlPanelType, SubControlType vSubCtrlType)
        {
            return true;
        }
    }

    public enum ControlPanelType
    {
        DebugControlPanel,
        CameraControlPanel,
        FeedbackControlPanel,
        RecordingPlaybackControlPanel,
        TagControlPanel,
        RenderedBodyControlPanel,
        SuitCommunicationControlPanel,
        CommentControlPanel,
        AnalysisControlPanel,
        MiscClientControlPanel,
        SettingControlPanel
    }

    public enum SubControlType
    {
        PlaybackSubControl,
        TagWidgetSubControl,
        TagModificationSubControl,
        TagTextBoxSubControl,
        CommentTextboxSubControl,
        CommentWidgetSubControl,
        RecordingSpeedModifierSubcontrol,
        PlaybackSliderSubcontrol,
        RecordingSelectionSubControl,
        SuitsCommunicationSubControl,
        SuitsSelectionSubControl,
        SuitConnectionSubControl,
        SuitModificationSubControl,
        RenderedBodyModifierSubControl,
        RenderedBodySelectionSubControl,
        ResetAvatarSubControl,
        AddTagSubControl
    }
}
