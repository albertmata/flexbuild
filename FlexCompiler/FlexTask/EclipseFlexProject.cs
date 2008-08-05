using System;
using System.Collections.Generic;
using System.Text;

using BuildTask.Flex.utils;
using System.IO;

namespace BuildTask.Flex
{
    public sealed class EclipseFlexProject
    {
        private string projectPath;

        public string ProjectPath
        {
            get { return projectPath; }
        }

        private ActionScriptProperties actionScriptProperties;

        public ActionScriptProperties ActionScriptProperties
        {
            get { return actionScriptProperties; }
        }

        private FlexPropertiesBase flexProperties;

        public FlexPropertiesBase FlexProperties
        {
            get { return flexProperties; }
        }

        private string projectName;

        public string ProjectName
        {
            get { return projectName; }
        }

        private FlexProjectType projectType;

        public FlexProjectType ProjectType
        {
            get { return projectType; }
        }

        private List<EclipseFlexProject> dependencies;

        public List<EclipseFlexProject> Dependencies
        {
            get { return dependencies; }
        }

        private Dictionary<string, string> linkedResources;

        public string ProjectOutputPath
        {
            get
            {
                string linkedResource = this.GetResourceByName(ActionScriptProperties.OutputFolderPath);
                if (null != linkedResource)
                    return linkedResource;
                else
                    return Path.Combine(ActionScriptProperties.OutputFolderLocation, ActionScriptProperties.OutputFolderPath);
            }
        }

        private string ProjectOutputFileExtension
        {
            get
            {
                if (ProjectType == FlexProjectType.FlexProject)
                {
                    return "swf";
                }
                else return "swc";
            }
        }

        public string ProjectOutputFile
        {
            get
            {
                return Path.Combine(ProjectOutputPath, string.Format("{0}.{1}", ProjectName, ProjectOutputFileExtension));
            }
        }

        private EclipseFlexProject()
        {
            dependencies = new List<EclipseFlexProject>(0);
            actionScriptProperties = new ActionScriptProperties();
            flexProperties = new FlexLibProperties();
        }

        public EclipseFlexProject(string path):this()
        {
            projectPath = path;
            using (ProjectReader reader = new ProjectReader(path))
            {
                projectName = reader.ProjectName;
                projectType = reader.ProjectType;
                linkedResources = reader.LinkedResources;
            }
        }

        public void LoadProperties(EclipseWorkspace wkspace)
        {
            if (projectType == FlexProjectType.FlexProject)
                flexProperties = new FlexProperties();
            else if (projectType == FlexProjectType.FlexLibraryProject)
                flexProperties = new FlexLibProperties(projectPath);
            actionScriptProperties = new ActionScriptProperties(projectPath, wkspace);
        }

        public void LoadDependencies(EclipseWorkspace workspace)
        {
            using (ProjectReader reader = new ProjectReader(projectPath))
            {
                string[] projectDependencies = reader.ProjectDependencies;
                foreach (string project in projectDependencies)
                {
                    EclipseFlexProject wkspaceProject = workspace.GetProjectByName(project);
                    if (null != wkspaceProject)
                        dependencies.Add(wkspaceProject);
                }
            }
        }

        public string GetResourceByName(string name)
        {
            if (linkedResources.ContainsKey(name))
            {
                return linkedResources[name];
            }
            return null;
        }
    }
}
