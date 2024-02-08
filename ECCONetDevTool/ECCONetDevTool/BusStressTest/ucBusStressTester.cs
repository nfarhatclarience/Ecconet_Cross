using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Diagnostics;

using ECCONet;
using static ECCONet.ECCONetApi;
using static ECCONet.Token.Keys;


namespace ECCONetDevTool.BusStressTest
{
    public partial class ucBusStressTester : UserControl
    {
        public ucBusStressTester()
        {
            //  initialize UI component
            InitializeComponent();
        }
    }
}
