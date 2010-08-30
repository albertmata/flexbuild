using System;
using System.Collections.Generic;
using System.Text;

using BuildTask.Flex.utils;
using System.IO;

namespace BuildTask.Flex
{
    public class ActionScriptProperties
    {
        //En caso que sea un proyecto de aplicación, cual es el mxml principal
        private string mainApplication;

        public string MainApplication
        {
            get { return mainApplication; }
        }

        //Raíz de las clases
        private string sourceFolderPath;

        public string SourceFolderPath
        {
            get { return sourceFolderPath; }
        }

        //Argumentos adicionales de compilación
        private string additionalCompilerArguments;

        public string AdditionalCompilerArguments
        {
            get { return additionalCompilerArguments; }
        }

        private string[] linkedLibraryPathEntries;

        public string[] LinkedLibraryPathEntries
        {
            get { return linkedLibraryPathEntries; }
        }

        private string[] externalLibraryPathEntries;

        public string[] ExternalLibraryPathEntries
        {
            get { return externalLibraryPathEntries; }
        }

        private string[] compilerSourcePathEntries;

        public string[] CompilerSourcePathEntries
        {
            get { return compilerSourcePathEntries; }
        }

        private string outputFolderLocation;

        public string OutputFolderLocation
        {
            get { return outputFolderLocation; }
        }
        private string outputFolderPath;

        public string OutputFolderPath
        {
            get { return outputFolderPath; }
        }

        private string playerVersion;

        public string PlayerVersion
        {
            get { return playerVersion; }
        }

        private bool verifyDigests;

        public bool VerifyDigests
        {
            get { return verifyDigests; }
        }

        private bool warn;

        public bool Warn
        {
            get { return warn; }
        }

        private string path;

        public ActionScriptProperties()
        {
            linkedLibraryPathEntries = new string[0];
            compilerSourcePathEntries = new string[0];
        }

        public ActionScriptProperties(string pathToProject, EclipseWorkspace wkspace)
        {
            path = pathToProject;
            using (ActionScriptPropertiesReader reader = new ActionScriptPropertiesReader(path, wkspace, wkspace.ProjectBaseDir, wkspace.NewBaseDir, wkspace.ReplacePaths))
            {
                mainApplication = reader.MainApplication;
                sourceFolderPath = reader.SourceFolderPath;
                if (string.IsNullOrEmpty(sourceFolderPath))
                {
                    sourceFolderPath = path;
                }
                else
                {
                    sourceFolderPath = Path.Combine(path, sourceFolderPath);
                }
                outputFolderLocation = reader.OutputFolderLocation;
                if (string.IsNullOrEmpty(outputFolderLocation))
                {
                    outputFolderLocation = pathToProject;
                }
                outputFolderPath = reader.OutputFolderPath;

                additionalCompilerArguments = reader.AdditionalCompilerArguments;
                linkedLibraryPathEntries = reader.LinkedLibraries;
                externalLibraryPathEntries = reader.ExternalLibraries;
                compilerSourcePathEntries = reader.CompilerSourcePaths;
                for (int i = 0; i < compilerSourcePathEntries.Length; i++)
                {
                    compilerSourcePathEntries[i] = Path.Combine(path, compilerSourcePathEntries[i]);
                }
                playerVersion = reader.HtmlPlayerVersion;
                verifyDigests = reader.VerifyDigests;
                warn = reader.Warn;
            }
        }
    }
}
