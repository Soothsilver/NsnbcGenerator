using System.Diagnostics;

namespace NsnbcGenerator
{
    internal static class NeocitiesPublisher
    {
        public static void PublishEverything()
        {
            string dir = "output";
            Process.Start(new ProcessStartInfo("neocities.bat", "push -e ncv output")
            {

            }).WaitForExit();
        }
    }
}