using System;
using System.Collections.Generic;
using System.Text;

using BuildTask.Flex.utils;
using System.IO;

namespace BuildTask.Flex
{
    public class FlexLibProperties : FlexPropertiesBase
    {
        private string[] includeClasses;

        public override string[] IncludeClasses
        {
            get { return includeClasses; }
        }

        private ProjectResource[] includeResources;

        public override ProjectResource[] IncludeResources
        {
            get { return includeResources; }
        }

        private string path;

        public FlexLibProperties()
        {
            includeClasses = new string[0];
            includeResources = new ProjectResource[0];
        }

        public FlexLibProperties(string pathToProject)
        {
            path = pathToProject;
            using (FlexLibPropertiesReader reader = new FlexLibPropertiesReader(path))
            {
                includeClasses = reader.IncludeClasses;
                includeResources = reader.IncludeResources;
                foreach (ProjectResource res in includeResources)
                {
                    res.SourcePath = Path.Combine(path, res.SourcePath);
                    //res.DestPath = Path.GetFileName(res.DestPath);
                }
            }
        }
    }
}
