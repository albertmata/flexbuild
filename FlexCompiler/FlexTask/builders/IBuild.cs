using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BuildTask.Flex.builders
{
    public interface IBuild
    {
        Process Build(EclipseFlexProject project, SwfMetaData metadata, bool debug, string outputFile);
    }
}
