using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex
{
    public class ProjectResource
    {
        private string destPath;

        public string DestPath
        {
            get { return destPath; }
            set { destPath = value; }
        }

        private string sourcePath;

        public string SourcePath
        {
            get { return sourcePath; }
            set { sourcePath = value; }
        }

        public ProjectResource(){}

        public ProjectResource(string destPath, string sourcePath)
        {
            this.destPath = destPath;
            this.sourcePath = sourcePath;
        }

        public override string ToString()
        {
            return sourcePath;
        }
    }
}
