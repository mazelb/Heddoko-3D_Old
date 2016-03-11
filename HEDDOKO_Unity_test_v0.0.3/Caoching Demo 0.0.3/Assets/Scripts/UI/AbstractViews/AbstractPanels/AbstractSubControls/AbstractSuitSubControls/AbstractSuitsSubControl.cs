using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Communication.Controller;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls
{
    public abstract class AbstractSuitsSubControl : AbstractSubControl
    {
        internal bool mIsConnectedToSuit;
        internal SuitState SuitState;
        public AbstractSuitConnection SuitConnection;
        
        void OnEnable()
        {
            //Listen to the AbstractSuitConnect OnConnect, OnDisconnect and OnStatusUpdate events
            SuitConnection.OnSuitStateUpdate += OnStatusUpdate;
            SuitConnection.ConnectedStateEvent += OnConnection;
            SuitConnection.DisconnectedStateEvent += OnDisconnect;
        }

        void OnDisable()
        {
            // ReSharper disable once DelegateSubtraction
            SuitConnection.OnSuitStateUpdate -= OnStatusUpdate;
            // ReSharper disable once DelegateSubtraction
            SuitConnection.ConnectedStateEvent -= OnConnection;
            // ReSharper disable once DelegateSubtraction
            SuitConnection.DisconnectedStateEvent -= OnDisconnect;
        }

        /// <summary>
        /// on suit disconnection
        /// </summary>
        public virtual void OnDisconnect()
        {
            mIsConnectedToSuit = false;
        }

        /// <summary>
        /// On status update
        /// </summary>
        /// <param name="vSuitState"></param>
        public abstract void OnStatusUpdate(SuitState vSuitState);

        /// <summary>
        /// on Connection
        /// </summary>
        public abstract void OnConnection();
         

    }
}
