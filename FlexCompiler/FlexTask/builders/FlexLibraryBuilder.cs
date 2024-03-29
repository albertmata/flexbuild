using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Resources;
using BuildTask.Flex.utils;

namespace BuildTask.Flex.builders
{
    public class FlexLibraryBuilder : IBuild
    {
        public Process Build(EclipseFlexProject project, SwfMetaData metadata, LicenseProperties license,
            bool debug, bool enableWarnings, string outputFile, out string finalOutputFile)
        {
            FlexLibProperties flexLibProp = project.FlexProperties as FlexLibProperties;
            ActionScriptProperties actionScriptProperties = project.ActionScriptProperties;

            string pathToXmlConfigFile = Path.Combine(project.ProjectPath, "project-config.xml");
            if (File.Exists(pathToXmlConfigFile))
            {
                try { File.Delete(pathToXmlConfigFile); }
                catch { }
            }

            //In libraries, ignore outputFile
            FlexConfigBuilder cfgBuilder = new FlexConfigBuilder();
            cfgBuilder.BuildConfigFile(pathToXmlConfigFile, flexLibProp, actionScriptProperties, metadata, license, debug, enableWarnings, project.ProjectOutputFile);
            finalOutputFile = project.ProjectOutputFile;

            string finalArgs = string.Format("-Xmx384m -Dsun.io.useCanonCaches=false -jar \"{0}/lib/compc.jar\" +flexlib=\"{0}/frameworks\" -load-config+=\"{1}\" {2}",
                FlexGlobals.FlexSdkPath, pathToXmlConfigFile, actionScriptProperties.AdditionalCompilerArguments);

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
