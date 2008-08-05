using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using BuildTask.Flex.builders;
using System.Diagnostics;

namespace BuildTask.Flex
{
    public class FlexBuild : Task
    {
        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.Normal, "Loading workspace");

                EclipseWorkspace wkspace = new EclipseWorkspace(WorkSpacePath);
                wkspace.LoadWorkspace();

                Log.LogMessage(MessageImportance.Normal, "Determining build order");
                
                EclipseFlexProject[] orderedProjects = wkspace.GetBuildOrder();
                SwfMetaData metadata = new SwfMetaData();
                metadata.creator = MetadataCreator;
                metadata.date = DateTime.Now.ToShortDateString();
                metadata.description = MetadataDescription;
                metadata.publisher = MetadataPublisher;
                metadata.title = MetadataTitle;
                for (int i = 0; i < orderedProjects.Length; i++)
                {
                    EclipseFlexProject project = orderedProjects[i];
                    
                    IBuild flexBuilder = FlexBuilderFactory.GetBuilderFromProject(project);
                    Log.LogMessage(MessageImportance.Normal, "Building project {0} to {1}", project.ProjectName, project.ProjectOutputPath);

                    using (Process p = flexBuilder.Build(project, metadata, Configurations.Debug == configuration, OutputFile))
                    {
                        p.WaitForExit(FlexGlobals.CompileTimeout);
                        Log.LogCommandLine(MessageImportance.Low, string.Format("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments));
                        if (p.HasExited)
                        {
                            if (p.ExitCode == 0)
                            {
                                Log.LogMessage(MessageImportance.High, p.StandardOutput.ReadToEnd());
                            }
                            else
                            {
                                Log.LogError(p.StandardError.ReadToEnd());
                            }
                        }
                    }
                }

                Log.LogMessage(MessageImportance.High, "Flex Project built successfully");
                
                return true;
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex, true);
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
            get { return FlexGlobals.FlexSdkPath; }
            set { FlexGlobals.FlexSdkPath = value; }
        }


        private Configurations configuration;
        [Required]
        [MonitoringDescription("Configuration (Debug or Release) mode in which we want to build")]
        public string Configuration
        {
            get { return configuration.ToString(); }
            set { configuration = (Configurations)Enum.Parse(typeof(Configurations),value); }
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
    }
}
