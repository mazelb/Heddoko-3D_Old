using UnityEngine;
using System.Collections;
using Assets.Scripts.Body_Data.view;
public class ButtonFunctonality : MonoBehaviour {

    public BodyView view;

	public void ResetJoint()
    {
        view.ResetInitialFrame();
    }
}
