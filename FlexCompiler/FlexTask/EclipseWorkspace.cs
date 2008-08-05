using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using BuildTask.Flex.utils;

namespace BuildTask.Flex
{
    public sealed class EclipseWorkspace
    {
        private const string pathToProjects = ".metadata/.plugins/org.eclipse.core.resources/.projects";
        private string pathToWorkspace;

        private Dictionary<string, EclipseFlexProject> projectsByName;

        public string PathToWorkspace
        {
            get { return pathToWorkspace; }
        }

        private List<EclipseFlexProject> projects;

        private EclipseFlexProject mainProject;

        public EclipseWorkspace()
        {
            projects = new List<EclipseFlexProject>(0);
            pathToWorkspace = string.Empty;
            projectsByName = new Dictionary<string, EclipseFlexProject>();
        }

        public EclipseWorkspace(string path)
            : this()
        {
            pathToWorkspace = path;
        }

        public void LoadWorkspace()
        {
            if (string.IsNullOrEmpty(pathToWorkspace))
                throw new ArgumentNullException("Path to workspace is not set");

            string fullPath = Path.Combine(pathToWorkspace, pathToProjects);
            if (!Directory.Exists(fullPath))
                throw new InvalidDataException("Path to workspace doesn't exists or is an invalid eclipse workspace");

            string[] dirs = Directory.GetDirectories(fullPath);

            if (0 == dirs.Length)
                throw new InvalidOperationException(string.Format("No projects exists in this workspace {0}",PathToWorkspace));

            foreach (string dir in dirs)
            {
                string dirName = Path.GetFileName(dir);
                EclipseFlexProject project = EclipseFlexProjectFactory.CreateProjectFromWorkspaceMetadata(dir, Path.Combine(pathToWorkspace, dirName));
                projects.Add(project);
                projectsByName.Add(project.ProjectName, project);
            }

            foreach (EclipseFlexProject project in this.projects)
            {
                //En principio solo hay uno
                if ( (project.ProjectType == FlexProjectType.FlexProject) && (null == mainProject) )
                    mainProject = project;
                project.LoadDependencies(this);
                project.LoadProperties(this);
            }
        }

        public EclipseFlexProject GetProjectByName(string name)
        {
            if(projectsByName.ContainsKey(name))
                return projectsByName[name];
            return null;
        }

        public EclipseFlexProject[] GetBuildOrder()
        {
            if (null == mainProject)
                throw new InvalidOperationException("There is no root project");

            return ProjectOrderBuilder.BuildProjectOrder(mainProject);
        }
    }
}
