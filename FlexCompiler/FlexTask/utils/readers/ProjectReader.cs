using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

namespace BuildTask.Flex.utils
{
    public class ProjectReader : IDisposable
    {
        private const string projectFileName = ".project";
        private const string actionScriptNature = "com.adobe.flexbuilder.project.actionscriptnature";
        private const string flexLibNature = "com.adobe.flexbuilder.project.flexlibnature";
        private const string flexNature = "com.adobe.flexbuilder.project.flexnature";

        private string path;

        private XmlDocument doc;

        public ProjectReader(string pathToProjectFile)
        {
            path = Path.Combine(pathToProjectFile, projectFileName);
            doc = new XmlDocument();
            doc.Load(path);
        }

        public string ProjectName
        {
            get
            {
                return doc.SelectSingleNode("//projectDescription/name/text()").Value;
            }
        }

        public FlexProjectType ProjectType
        {
            get
            {
                XmlNodeList nodeList = doc.SelectNodes("//projectDescription/natures/nature");

                foreach(XmlNode node in nodeList)
                {
                    if (flexLibNature == node.FirstChild.Value)
                        return FlexProjectType.FlexLibraryProject;
                    else if (flexNature == node.FirstChild.Value)
                        return FlexProjectType.FlexProject;
                }

                return FlexProjectType.ActionScriptLibraryProject;
            }
        }

        public string[] ProjectDependencies
        {
            get
            {
                XmlNodeList nodeList = doc.SelectNodes("//projectDescription/projects/project");
                ArrayList projectList = new ArrayList(nodeList.Count);

                foreach (XmlNode node in nodeList)
                {
                    projectList.Add(node.FirstChild.Value);    
                }

                return (string[])projectList.ToArray(typeof(string));
            }
        }

        public Dictionary<string, string> LinkedResources
        {
            get
            {
                XmlNodeList nodeList = doc.SelectNodes("//projectDescription/linkedResources/link");
                Dictionary<string, string> linkedResources = new Dictionary<string, string>(nodeList.Count);
                if (nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        try
                        {
                            string name = node.SelectSingleNode("name/text()").Value;
                            string location = node.SelectSingleNode("location/text()").Value;
                            linkedResources.Add(name, location);
                        }
                        catch
                        {
                        }
                    }
                }
                return linkedResources;
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
