using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BrainpackService.BrainpackServer
{
    public class BrainpackListener  
    {
        public Socket ListenerSocket { get; set; }

        public Guid BrainpackListenerGuid { get; } = Guid.NewGuid();

        public override bool Equals(object vObj)
        {
            if (vObj.GetType() != typeof (BrainpackListener))
            {
                return false;
            }
            BrainpackListener vBpListener = (BrainpackListener) vObj;
            return vBpListener.BrainpackListenerGuid == BrainpackListenerGuid;
        }

        public override int GetHashCode()
        {
            return BrainpackListenerGuid.GetHashCode();
        }
    }
}
