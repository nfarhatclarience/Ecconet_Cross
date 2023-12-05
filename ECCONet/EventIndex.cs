/**
  ******************************************************************************
  * @file    	EventIndex.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	M. Latham, Liquid Logic, LLC
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Event index manager.
  ******************************************************************************
  * @attention
  *
  * Unless required by applicable law or agreed to in writing, software created
  * by Liquid Logic, LLC is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES
  * OR CONDITIONS OF ANY KIND, either express or implied.
  *
  ******************************************************************************
  */
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCONet
{
    internal sealed class EventIndex
    {
        //  the event index
        byte eventIndex;


        /// <summary>
        /// Constructor.
        /// </summary>
        public EventIndex()
        {
            //	clear the index
            eventIndex = 0;
        }

        /// <summary>
        /// Returns the event index.
        /// </summary>
        /// <returns>The event index.</returns>
        public byte GetEventIndex()
        {
            if (0 == eventIndex)
                ++eventIndex;
            return eventIndex;
        }

        /// <summary>
        /// Bumps the event index.
        /// </summary>
        public void NextEventIndex()
        {
            ++eventIndex;
            if (0 == eventIndex)
                ++eventIndex;
        }

        /// <summary>
        /// Handles a new received event index.
        /// </summary>
        /// <param name="index">The index to analyze.</param>
        public void NewEventIndex(byte index)
        {
            sbyte result;

            //	incoming index of zero is ignored
            if (0 == index)
                return;

            //	if local index is zero (app just booted) or incoming index is newer,
            //	then update local index
            result = (sbyte)(index - eventIndex);
            if ((0 == eventIndex) || (result > 0))
                eventIndex = index;
        }

        /// <summary>
        /// Returns a value indicating whether the given event index has expired.
        /// </summary>
        /// <param name="index">The index to analyze.</param>
        /// <returns></returns>
        public bool IsEventIndexExpired(byte index)
        {
            //	incoming index of zero is never expired
            if (0 == index)
                return false;

            sbyte result = (sbyte)(index - eventIndex);
            return (result < 0);
        }

    }
}