using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using BuildTask.Flex.utils;

namespace BuildTask.Flex
{
    public class EclipseFlexProjectFactory
    {
        private const string locationFile = ".location";

        public static EclipseFlexProject CreateProjectFromWorkspaceMetadata(string dir, string wkspaceDir, string projectBaseDir, string newBaseDir, bool replacePaths)
        {
            string path = Path.Combine(dir, locationFile);
            if (File.Exists(path))
            {
                using (LocationFileReader reader = new LocationFileReader(path))
                {
                    string projectPath = String.Empty;
                    try
                    {
                        projectPath = reader.ReadProjectLocation();
                        return new EclipseFlexProject(projectPath, projectBaseDir, newBaseDir, replacePaths);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("An error has occurred while loading project", ex);
                    }
                }
            }
            else
            {
                return new EclipseFlexProject(wkspaceDir, projectBaseDir, newBaseDir, replacePaths);
            }
        }
    }
}
