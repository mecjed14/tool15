using System;
using System.Globalization;

namespace Buhler.IoT.Environment.ChangeLogTool.Helper
{
    public static class VersionHelper
    {
        public static Version ParseVersion(string major, string minor, string build)
        {
            var majorInt = int.Parse(major, CultureInfo.InvariantCulture);
            var minorInt = int.Parse(minor, CultureInfo.InvariantCulture);
            var buildInt = int.Parse(build, CultureInfo.InvariantCulture);
            return new  Version(majorInt, minorInt, buildInt);
        }
    }
}
