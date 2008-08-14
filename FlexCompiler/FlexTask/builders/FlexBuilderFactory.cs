using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex.builders
{
    public class FlexBuilderFactory
    {
        public static IBuild GetBuilderFromProject(EclipseFlexProject project)
        {
            if (project.ProjectType == FlexProjectType.FlexProject || project.ProjectType == FlexProjectType.ActionScriptLibraryProject)
            {
                return new FlexBuilder();
            }
            else
            {
                return new FlexLibraryBuilder();
            }
        }
    }
}
