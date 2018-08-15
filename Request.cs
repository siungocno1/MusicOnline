using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace MusicOnline
{
    class Request
    {

        //Kiểm tra kết nối
        [Flags]
        enum ConnectionInternetState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1, INTERNET_CONNECTION_LAN = 0x2, INTERNET_CONNECTION_PROXY = 0x4, INTERNET_RAS_INSTALLED = 0x10, INTERNET_CONNECTION_OFFLINE = 0x20, INTERNET_CONNECTION_CONFIGURED = 0x40
        }
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref ConnectionInternetState lpdwFlags, int dwReserved);
        public static bool IsConnected()
        {
            ConnectionInternetState Description = 0;
            bool conn = InternetGetConnectedState(ref Description, 0);
            return conn;
        }

        public static string cookies = "";
        public static string Send(string url, bool kt = false, string postData = "", bool onlyheader = false)
        {
            string html = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (postData == "")
                    request.Method = "GET";
                else
                    request.Method = "POST";
                if (kt == false)
                    request.AllowAutoRedirect = false;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) coc_coc_browser/53.2.131 Chrome/47.2.2526.131 Safari/537.36";

                //Thêm cookies
                if (cookies != "") request.Headers.Add("Cookie: " + cookies);
                Stream dataStream;
                if (postData != "")
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = byteArray.Length;
                    dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Flush();
                    dataStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string header = response.Headers.ToString();
                html = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                if (onlyheader) return header;
            }
            catch (Exception e) { html = "Lỗi: " + e.Message; }
            return html;
        }
        public static string GetKey()
        {
            for (int i = 'A'; i <= 'Z'; i++)
            {
                if (GetKey(Convert.ToChar(i).ToString()) != "0")
                    return GetKey(Convert.ToChar(i).ToString());
            }
            return "";
        }
        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);
        static uint serNum = 0;
        public static string GetKey(string strDriveLetter)
        {
            uint maxCompLen = 0;
            StringBuilder VolLabel = new StringBuilder(256); // Label
            UInt32 VolFlags = new UInt32();
            StringBuilder FSName = new StringBuilder(256); // File System Name
            strDriveLetter += ":\\"; // fix up the passed-in drive letter for the API call
            long Ret = GetVolumeInformation(strDriveLetter, VolLabel, (UInt32)VolLabel.Capacity, ref serNum, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);
            Console.WriteLine(serNum.ToString());
            return Convert.ToString(serNum);
        }
    }
}
