using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Collider2D))]
public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
  //  private Image mImage; //the image of the button
    //private Sprite mSprite;
    private RectTransform mCurrentRectTransform;
    private Collider2D mCollider2D;
    private InformationPanel mInformationPanel;
    public string InformationText;
    #region unity function

    void Awake()
    {
       // mImage = GetComponent<Image>();
        mCurrentRectTransform = GetComponent<RectTransform>();
        mCollider2D = GetComponent<Collider2D>();
        GameObject vInformationPanelGo = GameObject.FindGameObjectWithTag("InformationPanel");
        mInformationPanel = vInformationPanelGo.GetComponent<InformationPanel>();
    }
    #endregion


    public bool IsRaycastLocationValid(Vector2 vScreenPoint, Camera vEventCamera)
    {
        //mSprite = mImage.sprite;
        var worldPoint = Vector3.zero;
        var isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            mCurrentRectTransform,
            vScreenPoint,
            vEventCamera,
            out worldPoint
        );
        if (isInside)
        {
            isInside = mCollider2D.OverlapPoint(worldPoint);
            mInformationPanel.UpdateInformationPanel(InformationText);
        }
        return isInside;

    }
}
