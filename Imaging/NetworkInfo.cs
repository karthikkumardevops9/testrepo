using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Management;

namespace MSRecordsEngine.Imaging
{
    [DataContract()]
    public partial class NetworkInfo
    {
        public NetworkInfo()
        {
            // WCF proxy requires an empty constructor
        }

        public NetworkInfo(bool autoFill) : this()
        {
            if (!autoFill)
                return;
            ComputerName = //SystemInformation.ComputerName;
            DomainName = "web access";
            NetworkUserName = "web access";
            MACAddress = GetMACAddress();
            IPAddress = GetIPAddress();
        }

        public NetworkInfo(string ipAddress) : this()
        {
            MACAddress = GetMACAddress();
            IPAddress = ipAddress;
        }

        public NetworkInfo(string ipAddress, string macAddress) : this()
        {
            MACAddress = macAddress;
            IPAddress = ipAddress;
        }

        [DataMember()]
        public string ComputerName
        {
            get
            {
                return _computerName.Trim();
            }
            set
            {
                _computerName = value.Trim();
            }
        }
        private string _computerName = string.Empty;

        [DataMember()]
        public string DomainName
        {
            get
            {
                return _domainName.Trim();
            }
            set
            {
                _domainName = value.Trim();
            }
        }
        private string _domainName = string.Empty;

        [DataMember()]
        public string NetworkUserName
        {
            get
            {
                return _networkUserName.Trim();
            }
            set
            {
                _networkUserName = value.Trim();
            }
        }
        private string _networkUserName = string.Empty;

        [DataMember()]
        public string MACAddress
        {
            get
            {
                return _macAddress.Trim();
            }
            set
            {
                _macAddress = value.Trim();
            }
        }
        private string _macAddress = string.Empty;

        [DataMember()]
        public string IPAddress
        {
            get
            {
                return _ipAddress.Trim();
            }
            set
            {
                _ipAddress = value.Trim();
            }
        }
        private string _ipAddress = string.Empty;

        private string GetIPAddress()
        {
            string firstIP = string.Empty;

            try
            {
                var ips = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

                foreach (System.Net.IPAddress ip in ips)
                {
                    if (string.IsNullOrEmpty(firstIP))
                        firstIP = ip.ToString();
                    if ((ip.AddressFamily.ToString() ?? "") == (System.Net.Sockets.ProtocolFamily.InterNetwork.ToString() ?? ""))
                        return ip.ToString();
                }

                return firstIP;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return firstIP;
            }
        }

        private string GetMACAddress()
        {
            try
            {
                using (var oManagementClass = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    using (ManagementObjectCollection cManagementCollection = oManagementClass.GetInstances())
                    {
                        foreach (System.Management.ManagementObject oManagement in cManagementCollection)
                        {
                            if (string.Compare(oManagement["IPEnabled"].ToString(), "True", true) == 0)
                                return oManagement["MacAddress"].ToString();
                        }

                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
