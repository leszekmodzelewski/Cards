namespace GeoLib
{
    using System;
    using System.Management;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public static class LicenseManager
    {
        private static Task<bool> licenseTask = Task<bool>.Factory.StartNew(new Func<bool>(LicenseManager.CheckLicenseStatus));

        private static bool CheckLicenseStatus()
        {
            GetFingerprint();
            return true;
        }

        private static byte[] GetFingerprint()
        {
            byte[] buffer2;
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_Processor").Get().GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    string s = ((ManagementObject)enumerator.Current)["ProcessorId"].ToString();
                    byte[] bytes = Encoding.UTF8.GetBytes(s);
                    buffer2 = SHA256.Create().ComputeHash(bytes);
                }
                else
                {
                    return null;
                }
            }
            return buffer2;
        }

        public static bool IsLicenseValid =>
            licenseTask.Result;
    }
}

