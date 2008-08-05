using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;

namespace BuildTask.Flex.utils
{
    public class FlexLibPropertiesReader : IDisposable
    {
        private const string flexLibPropertiesFile = ".flexLibProperties";        

        private string path;

        private XmlDocument doc;

        public FlexLibPropertiesReader(string pathToProjectFile)
        {
            path = Path.Combine(pathToProjectFile, flexLibPropertiesFile);
            doc = new XmlDocument();
            doc.Load(path);
        }

        public string[] IncludeClasses
        {
            get
            {
                try
                {
                    XmlNodeList classNodes = doc.SelectNodes("//flexLibProperties/includeClasses/classEntry");
                    List<string> temp = new List<string>(classNodes.Count);
                    foreach (XmlNode node in classNodes)
                    {
                        temp.Add(node.Attributes["path"].Value);
                    }
                    return temp.ToArray();
                }
                catch
                {
                    return new string[0];
                }
            }
        }

        public ProjectResource[] IncludeResources
        {
            get
            {
                try
                {
                    XmlNodeList classNodes = doc.SelectNodes("//flexLibProperties/includeResources/resourceEntry");
                    List<ProjectResource> temp = new List<ProjectResource>(classNodes.Count);
                    foreach (XmlNode node in classNodes)
                    {
                        temp.Add(
                            new ProjectResource(
                                node.Attributes["destPath"].Value,
                                node.Attributes["sourcePath"].Value
                            )
                        );
                    }
                    return temp.ToArray();
                }
                catch
                {
                    return new ProjectResource[0];
                }
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
