using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECCONet
{
    public static class Output
    {
        public static void WriteLine(string message)
        {
            var messageWithTimeStamp = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}: {message}";
            Debug.WriteLine(messageWithTimeStamp);
            Console.WriteLine(messageWithTimeStamp);
        }
    }
}
