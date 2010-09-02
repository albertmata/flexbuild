using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using BuildTask.Flex.builders;
using System.Diagnostics;
using BuildTask.Flex.utils;

namespace BuildTask.Flex
{
    public class FlexBuild : Task
    {
        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.Normal, "Loading workspace from {0}", WorkSpacePath);
                Log.LogMessage(MessageImportance.Normal, "Replace base path {0} with {1} is {2}", ProjectsBasePath, NewBasePath, ReplaceProjectPaths?"Activated":"Deactivated");

                EclipseWorkspace wkspace = new EclipseWorkspace(WorkSpacePath, FlexUtil.NormalizePath(ProjectsBasePath), FlexUtil.NormalizePath(NewBasePath), ReplaceProjectPaths);
                wkspace.LoadWorkspace();

                Log.LogMessage(MessageImportance.Normal, "Determining build order");
                
                EclipseFlexProject[] orderedProjects = wkspace.GetBuildOrder();
                
                Log.LogMessage(MessageImportance.High, "Projects will be built in this order:");
                for (int i = 0; i < orderedProjects.Length;i++ )
                {
                    Log.LogMessage(MessageImportance.High, "[{0}] - {1}", i+1, orderedProjects[i].ProjectName);
                }

                SwfMetaData metadata = new SwfMetaData();
                metadata.creator = MetadataCreator;
                metadata.date = DateTime.Now.ToShortDateString();
                metadata.description = MetadataDescription;
                metadata.publisher = MetadataPublisher;
                metadata.title = MetadataTitle;

                LicenseProperties license = new LicenseProperties(LicenseProductName, LicenseSerial);

                string finalOutput;
                List<string> outputedFileList = new List<string>();

                Log.LogMessage(MessageImportance.Normal, "Java bin path is {0}", FlexGlobals.JavaBin);

                for (int i = 0; i < orderedProjects.Length; i++)
                {
                    EclipseFlexProject project = orderedProjects[i];
                    
                    IBuild flexBuilder = FlexBuilderFactory.GetBuilderFromProject(project);
                    Log.LogMessage(MessageImportance.High, "Building project {0} to {1}", project.ProjectName, project.ProjectOutputPath);

                    using (Process p = flexBuilder.Build(project, metadata, license, Configurations.Debug == configuration, EnableWarnings, OutputFile, out finalOutput))
                    {
                        p.WaitForExit(FlexGlobals.CompileTimeout);
                        outputedFileList.Add(finalOutput);
                        Log.LogCommandLine(MessageImportance.High, string.Format("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments));
                        if (p.HasExited)
                        {
                            if (p.ExitCode == 0)
                            {
                                Log.LogMessage(MessageImportance.High, p.StandardOutput.ReadToEnd());
                            }
                            else
                            {
                                Log.LogError(p.StandardError.ReadToEnd());
                                return false;
                            }
                        }
                        else
                        {
                            p.Kill();
                            Log.LogError(p.StandardError.ReadToEnd());
                            return false;
                        }
                    }
                }

                outputedFiles = outputedFileList.ToArray();

                Log.LogMessage(MessageImportance.High, "Flex Project built successfully");
                
                return true;
            }
            catch (Exception ex)
            {
                Exception theEx = ex;
                do
                {
                    this.Log.LogErrorFromException(theEx, true);
                    theEx = theEx.InnerException;
                } 
                while (theEx != null);
                return false;
            }
        }

        private string workSpacePath;
        [Required]
        [MonitoringDescription("Path to the eclipse workspace folder we want to compile")]
        public string WorkSpacePath
        {
            get { return workSpacePath; }
            set { workSpacePath = value; }
        }

        private string outputFile;
        [MonitoringDescription("Absolute path to the final generated swf, if not set the eclipse project path is used")]
        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        private string[] outputedFiles;
        [Output]
        public string[] OutputedFiles
        {
            get
            {
                return outputedFiles;
            }
        }

        [Required]
        [MonitoringDescription("Path to the Flex Sdk Folder, usually Program Files/Adobe/Flex/sdks/3.0.0")]
        public string FlexSdkPath
        {
            get { return FlexGlobals.FlexSdkPath; }
            set { FlexGlobals.FlexSdkPath = value; }
        }

        [Required]
        [MonitoringDescription("Path to the Java home folder")]
        public string JavaHome
        {
            get { return FlexGlobals.JavaHome; }
            set { FlexGlobals.JavaHome = value; }
        }


        private Configurations configuration = Configurations.Debug;
        [Required]
        [MonitoringDescription("Configuration (Debug or Release) mode in which we want to build")]
        public string Configuration
        {
            get { return configuration.ToString(); }
            set { configuration = (Configurations)Enum.Parse(typeof(Configurations),value); }
        }

        private bool enableWarnings = false;
        [MonitoringDescription("Enable strict warning mode")]
        public bool EnableWarnings
        {
            get { return enableWarnings; }
            set { enableWarnings = value; }
        }

        private string metadataCreator;
        [MonitoringDescription("Metadata inserted into SWF and SWC files")]
        public string MetadataCreator
        {
            get { return metadataCreator; }
            set { metadataCreator = value; }
        }
        
        private string metadataPublisher;
        [MonitoringDescription("Metadata inserted into SWF and SWC files")]
        public string MetadataPublisher
        {
            get { return metadataPublisher; }
            set { metadataPublisher = value; }
        }
        
        private string metadataTitle;
        [MonitoringDescription("Metadata inserted into SWF and SWC files")]
        public string MetadataTitle
        {
            get { return metadataTitle; }
            set { metadataTitle = value; }
        }

        private string metadataDescription;
        [MonitoringDescription("Metadata inserted into SWF and SWC files")]
        public string MetadataDescription
        {
            get { return metadataDescription; }
            set { metadataDescription = value; }
        }

        private string projectsBasePath = String.Empty;
        [MonitoringDescription("Path to the base projects folder")]
        public string ProjectsBasePath
        {
            get { return projectsBasePath; }
            set { projectsBasePath = value; }
        }

        private string newBasePath = String.Empty;
        [MonitoringDescription("Path to the new base projects folder")]
        public string NewBasePath
        {
            get { return newBasePath; }
            set { newBasePath = value; }
        }

        private bool replaceProjectPaths;
        [MonitoringDescription("Enable replace of projectsBasePath with newBasePath. Eclipse project paths stored in .location files are absolute.")]
        public bool ReplaceProjectPaths
        {
            get { return replaceProjectPaths; }
            set { replaceProjectPaths = value; }
        }

        private string licenseProductName = "flexbuilder3";
        [MonitoringDescription("ProductName for the license, normally flexbuilder3, do not touch if not necessary")]
        public string LicenseProductName
        {
            get { return licenseProductName; }
            set { licenseProductName = value; }
        }

        private string licenseSerial;
        [Required]
        [MonitoringDescription("Serial Number for the license")]
        public string LicenseSerial
        {
            get { return licenseSerial; }
            set { licenseSerial = value; }
        }
    }
}
