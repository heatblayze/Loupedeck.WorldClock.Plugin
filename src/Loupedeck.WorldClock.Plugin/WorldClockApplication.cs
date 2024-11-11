namespace Loupedeck.WorldClock
{
    using System;

    // This class can be used to connect the Loupedeck plugin to an application.

    public class WorldClockApplication : ClientApplication
    {
        public WorldClockApplication()
        {
        }

        // This method can be used to link the plugin to a Windows application.
        protected override String GetProcessName() => "";

        // This method can be used to link the plugin to a macOS application.
        protected override String GetBundleName() => "";
    }
}
