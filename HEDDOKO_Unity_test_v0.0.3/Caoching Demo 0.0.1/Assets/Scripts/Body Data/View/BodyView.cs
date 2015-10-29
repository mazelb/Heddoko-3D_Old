 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.view
{
    public class BodyView : MonoBehaviour
    {
        public Button ResetButton;
        private BodyFrameBuffer buffer;
        private Body associatedBody;
        public  bool StartUpdating { get; set; }

        public void Init(Body associatedBody, BodyFrameBuffer buffer)
        { 
            this.buffer = buffer;
            this.associatedBody = associatedBody; 
            
        }

        void OnDisable()
        {
            if (associatedBody != null)
            {
                associatedBody.StopThread();
            }
            
        }
        public void UpdateBuffer(BodyFrameBuffer buffer)
        {
            
        }


        private void Update()
        {
            if (StartUpdating)
            {
                if (buffer != null)
                {
                    BodyFrame bframe = buffer.Dequeue();
                    associatedBody.UpdateBody(bframe); 
                }
            }
          
        }

        public void ResetJoint()
        {
            associatedBody.SetInitialFrame(associatedBody.CurrentBodyFrame);
        }

    }
}
