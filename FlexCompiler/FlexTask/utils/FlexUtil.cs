using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex.utils
{
    public class FlexUtil
    {
        public static string NormalizePath(string path)
        {
            return path.Replace("\\\\", "/").Replace("\\", "/");
        }
        public static string UnNormalizePath(string path)
        {
            return path.Replace("/", "\\\\").Replace("/", "\\");
        }
    }
}
