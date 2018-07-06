namespace Logic
{
    public class DeviceInfo
    {
        /// <summary>
        /// 客户端版本
        /// </summary>
        public string version;
        /// <summary>
        /// 设备信息，如：iphone6
        /// </summary>
        public string device;
        /// <summary>
        /// 设备系统
        /// </summary>
        public OS os;
        /// <summary>
        /// 设备系统版本
        /// </summary>
        public string OSVersion;
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string IMEI;
        /// <summary>
        /// 设备物理地址
        /// </summary>
        public string MAC;
        /// <summary>
        /// 设备分辨率
        /// </summary>
        public string DPI;
        /// <summary>
        /// deviceId
        /// </summary>
        public string deviceId;
        public static string GetMacAddress()
        {
            string physicalAddress = "";
#if !UNITY_METRO || UNITY_EDITOR
            var nis = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            foreach (var ni in nis)
            {
                if (ni.Description == "en0")
                {
                    physicalAddress = ni.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = ni.GetPhysicalAddress().ToString();
                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }
#endif
            return physicalAddress;
        }
#if !Server
        public DeviceInfo()
        {
            try
            {
                //version = Client.Env.LocalClientVersion + "." + Client.Env.LocalGameVersion;

                DPI = UnityEngine.Screen.width + "*" + UnityEngine.Screen.height;
                OSVersion = UnityEngine.SystemInfo.operatingSystem;
                deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;

#if UNITY_EDITOR
                device = UnityEngine.SystemInfo.deviceModel;
                IMEI = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                MAC = GetMacAddress();
                os = OS.Windows;
#elif UNITY_ANDROID
                device = UnityEngine.SystemInfo.deviceModel;
                IMEI = "";
                MAC = GetMacAddress();
                os = OS.Android;//UnityEngine.Application.platform.ToString();
#elif UNITY_IOS
                device = UnityEngine.iOS.Device.generation.ToString();
                IMEI = "";
                MAC = GetMacAddress();
                os = OS.Ios;//UnityEngine.Application.platform.ToString();
#endif
            }
            catch { }
        }
#endif
    }
}