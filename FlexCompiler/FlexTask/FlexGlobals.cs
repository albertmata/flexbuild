using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BuildTask.Flex
{
    public class FlexGlobals
    {
        public static string FlexSdkPath = @"C:\Flex3SDK";
        //public static string FlexSdkPath = @"C:\Archivos de Programa\Adobe\Flex Builder 3 Plug-in\sdks\3.0.0";
        public static string JavaHome = @"C:\Archivos de Programa\Java\jre1.6.0_07";
        public static int CompileTimeout = 90000;

        public static string FlexFrameworkPath
        {
            get
            {
                return Path.Combine(FlexSdkPath, "frameworks");
            }
        }

        public static string FlexFrameworkLibPath
        {
            get
            {
                return Path.Combine(FlexSdkPath, "frameworks/libs");
            }
        }

        public static string FlexBinPath
        {
            get
            {
                return Path.Combine(FlexSdkPath, "bin");
            }
        }

        public static string FlexJarLibPath
        {
            get
            {
                return Path.Combine(FlexSdkPath, "lib");
            }
        }

        public static string FlexCompcPath
        {
            get
            {
                return Path.Combine(FlexSdkPath, "bin/compc.exe");
            }
        }

        public static string FlexMxmlcPath
        {
            get
            {
                return Path.Combine(FlexSdkPath, "bin/mxmlc.exe");
            }
        }

        public static string JavaBin
        {
            get
            {
                return Path.Combine(JavaHome, "bin/java.exe");
            }
        }
    }
}
