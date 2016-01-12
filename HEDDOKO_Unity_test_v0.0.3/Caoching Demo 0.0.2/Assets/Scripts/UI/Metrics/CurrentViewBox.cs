
/**  
* @file CurrentViewBox.cs 
* @brief Contains the CurrentViewBox class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Displays the information on the current limbs/segments
    /// </summary>
   public class CurrentViewBox : MonoBehaviour
   {
       public Text SideBoxText;
       public Text AnalysingText;
        public BikingMetricsView BikingMetrics; 
        public DualPurposeMetricsView DualPurposeMetrics;
        public void UpdateText(bool vIn2DView, int vNewPos)
        {
            if (vIn2DView)
            {
                SideBoxText.text = "SIDE VIEW";
                if (vNewPos == 0)
                {
                    AnalysingText.text = "LEFT LEG";
                    BikingMetrics.DisplayRightLegAnalysis = false;
                    DualPurposeMetrics.DisplayRightLegAnalysis = false;
                }
                else
                {
                    AnalysingText.text = "RIGHT LEG";
                    BikingMetrics.DisplayRightLegAnalysis = true;
                    DualPurposeMetrics.DisplayRightLegAnalysis = true;
                }
            }
            else
            {
                if (vNewPos == 0)
                {
                    SideBoxText.text = "FRONT VIEW"; 
                }
                if (vNewPos == 1)
                {
                    SideBoxText.text = "LEFT VIEW";
                    AnalysingText.text = "LEFT LEG";
                    BikingMetrics.DisplayRightLegAnalysis = false;
                    DualPurposeMetrics.DisplayRightLegAnalysis = false;
                }
                if (vNewPos == 2)
                {
                    SideBoxText.text = "BACK VIEW";
                  
                }
                if (vNewPos == 3)
                {
                    SideBoxText.text = "RIGHT VIEW";
                    AnalysingText.text = "RIGHT LEG";
                    BikingMetrics.DisplayRightLegAnalysis = true;
                    DualPurposeMetrics.DisplayRightLegAnalysis = true;
                }
            }
        }

   }
}
