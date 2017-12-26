namespace GatewayServer.Air.Configuration
{
    using System.Runtime.InteropServices;

    class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = false)]
        public static extern int GetPrivateProfileSectionNames([Out, MarshalAs(UnmanagedType.LPArray)] char[] lpszReturnBuffer, int nSize, [In, MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = false)]
        public static extern int GetPrivateProfileSection(string lpAppName, [MarshalAs(UnmanagedType.VBByRefStr)]ref string lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = false)]
        public static extern int WritePrivateProfileString(string section, string key, string value, string files);
    }
}
