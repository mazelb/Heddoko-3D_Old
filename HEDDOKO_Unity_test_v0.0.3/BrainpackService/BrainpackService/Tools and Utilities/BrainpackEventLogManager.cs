using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace BrainpackService.Tools_and_Utilities
{
   public  class BrainpackEventLogManager
   {
       public  delegate void ExceptionHandlingDelegate(string vMsg);

        public delegate void ExceptionThrowingDelegate(string vMsg);

       public delegate void EventLogDelegate(string vMsg);
        private static event ExceptionThrowingDelegate ExceptionThrowingDelegateHandler;
        private static event ExceptionHandlingDelegate ExceptionHandlingDelegateHandler;
       private static event EventLogDelegate EventLogHandling;

       public static void RegisterExceptionHandlingDelegate(ExceptionHandlingDelegate vEHD)
       {
           ExceptionHandlingDelegateHandler += vEHD;
       }

       public static void RegisterExceptionThrowingDelegate(ExceptionThrowingDelegate vETD)
       {
           ExceptionThrowingDelegateHandler += vETD;
       }

        public static void DeRegisterExceptionHandlingDelegate(ExceptionHandlingDelegate vEHD)
        {
            ExceptionHandlingDelegateHandler -= vEHD;
        }

        public static void DeRegisterExceptionThrowingDelegate(ExceptionThrowingDelegate vETD)
        {
            ExceptionThrowingDelegateHandler -= vETD;
        }

       public static void RegisterEventLogMessage(EventLogDelegate vELD)
       {
           EventLogHandling += vELD;
       }

        public static void DeRegisterEventLogMessage(EventLogDelegate vELD)
        {
            EventLogHandling -= vELD;
        }
        public static void InvokeExceptionThrowingDelegate(string msg)
       {
           if (ExceptionThrowingDelegateHandler != null)
           {
               ExceptionThrowingDelegateHandler(msg);
           }
       }

       public static void InvokeEventLogMessage(string msg)
       {
           if (EventLogHandling != null)
           {
               EventLogHandling(msg);
           }
       }

       public static void InvokeEventLogError(string msg)
       {
            if (EventLogHandling != null)
            {
                EventLogHandling(msg);
            }
        }

       public static void InvokeNetworkingException(string msg)
       {
           
       }
    }
}
