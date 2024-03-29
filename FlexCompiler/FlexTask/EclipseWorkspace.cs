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

        private string projectBaseDir;
        private string newBaseDir;
        private bool replacePaths;

        private Dictionary<string, EclipseFlexProject> projectsByName;

        public string PathToWorkspace
        {
            get { return pathToWorkspace; }
        }
        public string ProjectBaseDir
        {
            get { return projectBaseDir; }
        }
        public string NewBaseDir
        {
            get { return newBaseDir; }
        }
        public bool ReplacePaths
        {
            get { return replacePaths; }
        }
        
        private List<EclipseFlexProject> projects;

        private EclipseFlexProject mainProject;

        public EclipseWorkspace()
        {
            projects = new List<EclipseFlexProject>(0);
            pathToWorkspace = string.Empty;
            projectsByName = new Dictionary<string, EclipseFlexProject>();
            projectBaseDir = string.Empty;
            newBaseDir = string.Empty;
            replacePaths = false;
        }

        public EclipseWorkspace(string path, string projectBaseDir, string newBaseDir, bool replacePaths)
            : this()
        {
            pathToWorkspace = path;
            this.replacePaths = replacePaths;
            this.projectBaseDir = projectBaseDir;
            this.newBaseDir = newBaseDir;
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
                if(dir.EndsWith(".svn")) continue;
                string dirName = Path.GetFileName(dir);
                EclipseFlexProject project = EclipseFlexProjectFactory.CreateProjectFromWorkspaceMetadata(dir, Path.Combine(pathToWorkspace, dirName),this.projectBaseDir, this.newBaseDir, this.replacePaths);
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

            if (null == mainProject)
            {
                //Search for ActionScriptProjects
                foreach (EclipseFlexProject project in this.projects)
                {
                    //En principio solo hay uno
                    if ((project.ProjectType == FlexProjectType.ActionScriptLibraryProject) && (null == mainProject))
                    {
                        mainProject = project;
                        break;
                    }
                }
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

            EclipseFlexProject[] orderedProjects = ProjectOrderBuilder.BuildProjectOrder(mainProject);
            
            //No estan todos los proyectos en la lista? Vamos a a�adirlos todos
            if (orderedProjects.Length != projects.Count)
            {
                return projects.ToArray();
            }
            return orderedProjects;
        }
    }
}
