using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace BuildTask.Flex.utils
{
    public class ActionScriptPropertiesReader : IDisposable
    {
        private const string actionScriptFilename = ".actionScriptProperties";

        private const string frameWorkVar = "${PROJECT_FRAMEWORKS}";
        private string path;
        private string projectsBasePath;
        private string newBasePath;
        private bool replacePaths;

        private XmlDocument doc;

        private EclipseWorkspace workSpace;

        public ActionScriptPropertiesReader(string pathToProject, EclipseWorkspace workspace, string projectBaseDir, string newBaseDir, bool replacePaths)
        {
            workSpace = workspace;
            path = Path.Combine(pathToProject, actionScriptFilename);
            doc = new XmlDocument();
            doc.Load(path);
            projectsBasePath = projectBaseDir;
            newBasePath = newBaseDir;
            this.replacePaths = replacePaths;
        }

        public string MainApplication
        {
            get
            {
                return doc.SelectSingleNode("//actionScriptProperties").Attributes["mainApplicationPath"].Value;
            }
        }

        public string GetCompilerAttribute(string key)
        {
            XmlNode node = doc.SelectSingleNode("//actionScriptProperties/compiler");
            if (null != node.Attributes[key])
            {
                return node.Attributes[key].Value;
            }
            else
                return string.Empty;
        }

        public string SourceFolderPath
        {
            get
            {
                return GetCompilerAttribute("sourceFolderPath");
            }
        }

        public string AdditionalCompilerArguments
        {
            get
            {
                return GetCompilerAttribute("additionalCompilerArguments");
            }
        }

        public string OutputFolderPath
        {
            get
            {
                return GetCompilerAttribute("outputFolderPath");
            }
        }
        
        public string OutputFolderLocation
        {
            get
            {
                path = GetCompilerAttribute("outputFolderLocation");
                if (replacePaths)
                {
                    path = path.Replace(projectsBasePath, newBasePath);
                }
                return path;
            }
        }

        public string HtmlPlayerVersion
        {
            get
            {
                return GetCompilerAttribute("htmlPlayerVersion");
            }
        }

        public bool VerifyDigests
        {
            get
            {
                bool verify = false;
                Boolean.TryParse(GetCompilerAttribute("verifyDigests"), out verify);
                return verify;
            }
        }

        public bool Warn
        {
            get
            {
                bool warn = false;
                Boolean.TryParse(GetCompilerAttribute("warn"), out warn);
                return warn;
            }
        }

        public string[] LinkedLibraries
        {
            get
            {
                return ProcessLibraries(FlexLibraryLinkType.Merged);
            }
        }

        public string[] ExternalLibraries
        {
            get
            {
                return ProcessLibraries(FlexLibraryLinkType.External);
            }
        }

        private string[] ProcessLibraries(FlexLibraryLinkType type)
        {
            try
            {
                XmlNode libraryPath = doc.SelectSingleNode("//actionScriptProperties/compiler/libraryPath");
                FlexLibraryLinkType defaultLinkType;
                try
                {
                    defaultLinkType = (FlexLibraryLinkType)Enum.Parse(typeof(FlexLibraryLinkType), libraryPath.Attributes["defaultLinkType"].Value);
                }
                catch
                {
                    defaultLinkType = FlexLibraryLinkType.Merged;
                }

                List<string> libraries = new List<string>();
                foreach (XmlNode child in libraryPath.ChildNodes)
                {                 
                    ProcessLibraryPathEntry(type, child, defaultLinkType, libraries);
                }
                if (type == FlexLibraryLinkType.External)
                {
                    libraries.Add(FlexUtil.NormalizePath(Path.Combine(FlexGlobals.FlexSdkPath, @"frameworks\libs\player\playerglobal.swc")));
                }

                return libraries.ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        private void ProcessLibraryPathEntry(FlexLibraryLinkType type, XmlNode libraryPathEntryNode, FlexLibraryLinkType defaultLinkType, List<string> libraries)
        {
            bool useDefaultLinkType = false;
            if (libraryPathEntryNode.Attributes["useDefaultLinkType"] != null)
            {
                Boolean.TryParse(libraryPathEntryNode.Attributes["useDefaultLinkType"].Value, out useDefaultLinkType);
            }

            FlexLibraryKind kind = (FlexLibraryKind)Enum.Parse(typeof(FlexLibraryKind), libraryPathEntryNode.Attributes["kind"].Value);

            if (null == libraryPathEntryNode.Attributes["linkType"])
                useDefaultLinkType = true;
            FlexLibraryLinkType linkType = useDefaultLinkType ? defaultLinkType : (FlexLibraryLinkType)Enum.Parse(typeof(FlexLibraryLinkType), libraryPathEntryNode.Attributes["linkType"].Value);

            string path = libraryPathEntryNode.Attributes["path"].Value;
            path = path.Replace(frameWorkVar, FlexGlobals.FlexFrameworkPath);
            if (replacePaths)
            {
                path = path.Replace(projectsBasePath, newBasePath);
            }

            switch (kind)
            {
                case FlexLibraryKind.SwcFolder:
                    if (type == linkType)
                    {
                        ProcessLibraryFolder(FlexUtil.NormalizePath(path), libraries, type, libraryPathEntryNode, defaultLinkType);
                    }
                    break;
                case FlexLibraryKind.SwcFile:
                    if (type == linkType)
                    {
                        if (path.IndexOf(Path.VolumeSeparatorChar) != -1)
                        {
                            libraries.Add(path);
                        }
                        else
                        {
                            libraries.Add(ConvertToAbsolute(path));
                        }
                    }
                    break;
                case FlexLibraryKind.FlexLibs:
                    useDefaultLinkType = true;
                    ProcessLibraryFolder(FlexUtil.NormalizePath(FlexGlobals.FlexFrameworkLibPath), libraries, type, libraryPathEntryNode, defaultLinkType);
                    if (type == FlexLibraryLinkType.Merged)
                    {
                        libraries.Add(FlexUtil.NormalizePath(Path.Combine(FlexGlobals.FlexSdkPath, @"frameworks\locale\{locale}")));
                    }
                    break;
                default:
                    break;
            }
        }

        private string ConvertToAbsolute(string path)
        {
            if (path.StartsWith("/"))
            {
                string tempPath = path.Substring(1, path.Length-1);
                string projectName = tempPath.Substring(0, tempPath.IndexOf("/"));
                EclipseFlexProject project = workSpace.GetProjectByName(projectName);
                if (null != project)
                {
                    return Path.Combine(project.ProjectPath, tempPath.Substring(tempPath.IndexOf("/") + 1));
                }
                return FlexUtil.NormalizePath(path);
            }
            return path;
        }

        private void ProcessLibraryFolder(string directory, List<string> libraries, FlexLibraryLinkType type, XmlNode libraryNode, FlexLibraryLinkType defaultLinkType)
        {
            List<string> excludedPaths = new List<string>();
            Dictionary<string, XmlNode> modifiedPaths = new Dictionary<string, XmlNode>();
            if (libraryNode.HasChildNodes)
            {
                XmlNodeList excludedLibs = libraryNode.SelectNodes("excludedEntries/libraryPathEntry");
                if (excludedLibs.Count > 0)
                {
                    foreach (XmlNode node in excludedLibs)
                    {
                        string path = node.Attributes["path"].Value;
                        path = path.Replace(frameWorkVar, FlexGlobals.FlexFrameworkPath);
                        path = FlexUtil.NormalizePath(path);
                        excludedPaths.Add(path);
                    }
                }

                XmlNodeList modifiedLibs = libraryNode.SelectNodes("modifiedEntries/libraryPathEntry");
                if (modifiedLibs.Count > 0)
                {
                    foreach (XmlNode node in modifiedLibs)
                    {
                        string path = node.Attributes["path"].Value;
                        path = path.Replace(frameWorkVar, FlexGlobals.FlexFrameworkPath);
                        path = FlexUtil.NormalizePath(path);
                        modifiedPaths.Add(path, node);
                    }
                }
            }

            string [] swcfiles = Directory.GetFiles(directory);
            foreach (string swcFile in swcfiles)
            {
                string normFile = FlexUtil.NormalizePath(swcFile);
                if (!excludedPaths.Contains(normFile))
                {
                    if (modifiedPaths.ContainsKey(normFile))
                    {
                        ProcessLibraryPathEntry(type, modifiedPaths[normFile], defaultLinkType, libraries);
                    }
                    else
                    {
                        if(defaultLinkType == type)
                            libraries.Add(normFile);
                    }
                }
            }
        }

        public string[] CompilerSourcePaths
        {
            get
            {
                XmlNodeList nodeList = doc.SelectNodes("//actionScriptProperties/compiler/compilerSourcePath/compilerSourcePathEntry");
                List<string> list = new List<string>();
                if (nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        list.Add(node.Attributes["path"].Value);
                    }
                }
                return list.ToArray();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (null != doc)
                doc = null;
        }

        #endregion
    }
}
