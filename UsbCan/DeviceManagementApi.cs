/// <summary>
/// Project: WinUsb_cs
/// 
/// ***********************************************************************
/// Software License Agreement
///
/// Licensor grants any person obtaining a copy of this software ("You") 
/// a worldwide, royalty-free, non-exclusive license, for the duration of 
/// the copyright, free of charge, to store and execute the Software in a 
/// computer system and to incorporate the Software or any portion of it 
/// in computer programs You write.   
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// ***********************************************************************
/// 
/// Author
/// Jan Axelson          
/// 
/// This software was created using Visual Studio 2012 Professional Edition with .NET Framework 4.0.


using System;
using System.Runtime.InteropServices;


namespace JanAxelsonWinUSB
{
    ///<summary >
    // API declarations relating to device management (SetupDixxx and 
    // RegisterDeviceNotification functions).   
    /// </summary>

    sealed internal partial class DeviceManagement
    {
        internal static class NativeMethods
        {
            // from setupapi.h

            internal const Int32 DIGCF_PRESENT = 2;
            internal const Int32 DIGCF_DEVICEINTERFACE = 0X10;

            internal struct SP_DEVICE_INTERFACE_DATA
            {
                internal Int32 cbSize;
                internal Guid InterfaceClassGuid;
                internal Int32 Flags;
                internal IntPtr Reserved;
            }

            /* Mark Latham commented these two structures out.
             * They are not used anywhere.
            internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
            {
                internal Int32 cbSize;
                internal String DevicePath;
            }

            internal struct SP_DEVINFO_DATA
            {
                internal Int32 cbSize;
                internal Guid ClassGuid;
                internal Int32 DevInst;
                internal Int32 Reserved;
            }
            */

            [DllImport("setupapi.dll", SetLastError = true)]
            internal static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hwndParent);

            [DllImport("setupapi.dll", SetLastError = true)]
            internal static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

            [DllImport("setupapi.dll", SetLastError = true)]
            internal static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, Int32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);


        }
    }
}
