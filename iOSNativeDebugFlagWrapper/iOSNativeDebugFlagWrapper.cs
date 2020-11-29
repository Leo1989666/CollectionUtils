namespace iOSNativeDebug
{
    using System.Runtime.InteropServices;

    public static class iOSNativeDebugFlagWrapper
    {
        [DllImport("__Internal")]
        public static extern bool GetEnableDebugLogFlag();

        [DllImport("__Internal")]
        public static extern bool GetEnableSRDebuggerFlag();

        [DllImport("__Internal")]
        public static extern bool GetEnableAnalyticsFlag();
    }    
}
