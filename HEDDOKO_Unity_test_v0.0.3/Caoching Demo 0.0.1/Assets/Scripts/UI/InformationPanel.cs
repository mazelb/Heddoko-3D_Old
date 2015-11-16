using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
public class InformationPanel : MonoBehaviour
{

    public Text mDisplayText;

    private Text DisplayText
    {
        get
        {
            if (mDisplayText == null)
            {
                mDisplayText = GetComponent<Text>();
            }
            return mDisplayText;
        }
    }
   
    public void UpdateInformationPanel(string mMessage)
    {
        DisplayText.text = mMessage;
    }
}
