using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Interfaces
{
    /** 
    * @file Body.cs
    * @brief Contains the IFrameStream interface
    * @author Mohammed Haider(mohammed@heddoko.com)
    * @date October 2015
    */
    internal interface IFrameStream
    {
        /**
        * GetNextFrame()
        * @param 
        * @brief Returns the next frame from the stream 
        * @return Returns the next frame from the stream 
        */
        BodyRawFrame GetNextFrame();
    }
}
