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
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace JanAxelsonWinUSB
{
    ///  <summary>
    ///  API declarations relating to file I/O (and used by WinUsb).
    ///  </summary>

    sealed internal class FileIo
    {
        internal static class NativeMethods
        {
            internal const Int32 FILE_ATTRIBUTE_NORMAL = 0X80;
            internal const Int32 FILE_FLAG_OVERLAPPED = 0X40000000;
            internal const Int32 FILE_SHARE_READ = 1;
            internal const Int32 FILE_SHARE_WRITE = 2;
            internal const UInt32 GENERIC_READ = 0X80000000;
            internal const UInt32 GENERIC_WRITE = 0X40000000;
            internal const Int32 INVALID_HANDLE_VALUE = -1;
            internal const Int32 OPEN_EXISTING = 3;

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        }
    }

}
