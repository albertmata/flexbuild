using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace imaxel.BuildTask.Flex.builders
{
    public class FlexLibraryBuilder : IBuild
    {
        public Process Build(EclipseFlexProject project, SwfMetaData metadata)
        {
            StringBuilder compcParameters = new StringBuilder();
            compcParameters.AppendFormat("-compiler.source-path {0}", project.ActionScriptProperties.SourceFolderPath);
            compcParameters.Append(" ");
            
            FlexLibProperties flexLibProp = project.FlexProperties as FlexLibProperties;
            ActionScriptProperties actionScriptProperties = project.ActionScriptProperties;
            //Resources
            if (flexLibProp.IncludeResources.Length > 0)
            {
                compcParameters.Append(GetMultiFiles(flexLibProp.IncludeResources));
                compcParameters.Append(" ");
            }
            //Compiler source paths
            if (actionScriptProperties.CompilerSourcePathEntries.Length > 0)
            {
                compcParameters.Append(GetMultiParam("-compiler.source-path", actionScriptProperties.CompilerSourcePathEntries));
                compcParameters.Append(" ");
            }
            //Linked Libraries
            if (actionScriptProperties.LinkedLibraryPathEntries.Length > 0)
            {
                compcParameters.Append(GetMultiParam("-compiler.library-path", actionScriptProperties.LinkedLibraryPathEntries));
                compcParameters.Append(" ");
            }
            //External libraries
            if (actionScriptProperties.ExternalLibraryPathEntries.Length > 0)
            {
                compcParameters.Append(GetMultiParam("-compiler.external-library-path", actionScriptProperties.ExternalLibraryPathEntries));
                compcParameters.Append(" ");
            }
            //Metadata
            compcParameters.Append(metadata.ToString());
            compcParameters.Append(" ");
            //Output file
            compcParameters.AppendFormat("-output {0}", project.ProjectOutputFile);
            compcParameters.Append(" ");
            //Additional parameters
            compcParameters.Append(actionScriptProperties.AdditionalCompilerArguments);
            compcParameters.Append(" ");
            compcParameters.AppendFormat("-warnings={0} -target-player {1} ", actionScriptProperties.Warn,actionScriptProperties.PlayerVersion);
            compcParameters.Append("-compiler.optimize=true -compiler.debug=false -- ");
            //Classes
            compcParameters.Append(GetIncludeClasses(flexLibProp.IncludeClasses));

            string finalArgs = string.Format("-Xmx384m -Dsun.io.useCanonCaches=false -jar \"{0}/lib/compc.jar\" +flexlib=\"{0}/frameworks\" {1}", 
                FlexGlobals.FlexSdkPath, compcParameters.ToString());

            //Try to cleanup
            if (File.Exists(project.ProjectOutputFile))
            {
                try
                {
                    File.Delete(project.ProjectOutputFile);
                }
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

        private string GetMultiFiles(ProjectResource[] projectResource)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ProjectResource pr in projectResource)
            {
                sb.AppendFormat("-include-file {0} {1} ", EntreComilla(NormalizaPath(pr.DestPath)), EntreComilla(NormalizaPath(pr.SourcePath)));
            }
            return sb.ToString();
        }

        private string GetMultiParam(string param, string[] files)
        {
            string[] entrecomillados = new string[files.Length];
            int i=0;
            foreach (string file in files)
            {
                entrecomillados[i++] = EntreComilla(NormalizaPath(file));
            }
            return string.Format("{0}+={1}",param, String.Join(",", entrecomillados));   
        }

        private string GetIncludeClasses(string[] classes)
        {
            return String.Join(" ", classes);
        }

        private string EntreComilla(string input)
        {
            return string.Format("\"{0}\"", input);
        }

        private string NormalizaPath(string path)
        {
            return path.Replace("\\\\", "/").Replace("\\", "/");
        }
    }
}
