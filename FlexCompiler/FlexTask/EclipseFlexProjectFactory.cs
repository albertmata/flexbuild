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

        public static EclipseFlexProject CreateProjectFromWorkspaceMetadata(string dir, string wkspaceDir)
        {
            string path = Path.Combine(dir, locationFile);
            if (File.Exists(path))
            {
                using (LocationFileReader reader = new LocationFileReader(path))
                {
                    try
                    {
                        string projectPath = reader.ReadProjectLocation();
                        return new EclipseFlexProject(projectPath);
                    }
                    catch
                    {
                        try
                        {
                            return new EclipseFlexProject(wkspaceDir);
                        }
                        catch
                        {
                            throw new InvalidOperationException("An error has occurred while loading project");
                        }
                    }
                }
            }
            else
            {
                return new EclipseFlexProject(wkspaceDir);
            }
            return null;
        }
    }
}
