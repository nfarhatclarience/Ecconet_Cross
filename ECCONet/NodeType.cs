/**
  ******************************************************************************
  * @file    	NodeType.cs
  * @copyright  © 2017 ECCO Group.  All rights reserved.
  * @author  	Dustin Christopherson.
  * @version 	1.0.0
  * @date    	April 2017
  * @brief   	Matrix library configuration.
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
    public enum NodeTypes
    {
        NodeTypeUnknown = 0,
        NoNeighbor = 0,
        Ui,   
        Siren,
        Network,
        ControlHead,
        ControlHeadRotary,
        Sib,
        Lightbar,

        //	other standard node types will start at 20
        Supervisor = 20,
        Wingman,
        Citadel,
        SwitchNode,

        //	waiting identification used for Serial Lightbar and Serial Siren system
        WaitingIdentification = 0xff,
    }
}
