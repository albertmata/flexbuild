using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using BuildTask.Flex.utils;

namespace BuildTask.Flex.builders
{
    public class FlexBuilder : IBuild
    {
        public Process Build(EclipseFlexProject project, SwfMetaData metadata, LicenseProperties license, bool debug, bool enableWarnings, string outputFile, out string finalOutputFile)
        {
            FlexPropertiesBase flexLibProp = project.FlexProperties;
            ActionScriptProperties actionScriptProperties = project.ActionScriptProperties;

            string pathToXmlConfigFile = Path.Combine(project.ProjectPath, "project-config.xml");
            if (File.Exists(pathToXmlConfigFile))
            {
                try { File.Delete(pathToXmlConfigFile); }
                catch { }
            }

            FlexConfigBuilder cfgBuilder = new FlexConfigBuilder();
            if (string.IsNullOrEmpty(outputFile))
            {
                outputFile = project.ProjectOutputFile;

                //Hack: El proyecto de flex tiene por defecto el ouputfolder a bin-debug, pero nosotros queremos que si estamos en release
                //nos lo ponga a bin-release
                if (!debug && outputFile.Contains("bin-debug"))
                {
                    outputFile = outputFile.Replace("bin-debug", "bin-release");
                }
            }

            finalOutputFile = outputFile;

            cfgBuilder.BuildConfigFile(pathToXmlConfigFile, flexLibProp, actionScriptProperties, metadata, license, debug, enableWarnings, outputFile);

            string pathToMainApp = FlexUtil.NormalizePath(Path.Combine(project.ProjectPath, actionScriptProperties.MainApplication));

            string finalArgs = string.Format("-Xmx384m -Dsun.io.useCanonCaches=false -jar \"{0}/lib/mxmlc.jar\" +flexlib=\"{0}/frameworks\" -load-config+=\"{1}\" {2} {3}",
                FlexGlobals.FlexSdkPath, pathToXmlConfigFile, actionScriptProperties.AdditionalCompilerArguments, pathToMainApp);

            //Try to cleanup
            if (File.Exists(project.ProjectOutputFile))
            {
                try { File.Delete(project.ProjectOutputFile); }
                catch { }
            }
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(FlexGlobals.JavaBin, finalArgs);
            p.StartInfo.WorkingDirectory = FlexGlobals.FlexBinPath;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            return p;
        }
    }
}
