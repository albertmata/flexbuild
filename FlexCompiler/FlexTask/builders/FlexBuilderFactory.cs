using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex.builders
{
    public class FlexBuilderFactory
    {
        public static IBuild GetBuilderFromProject(EclipseFlexProject project)
        {
            if (project.ProjectType == FlexProjectType.FlexProject)
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
