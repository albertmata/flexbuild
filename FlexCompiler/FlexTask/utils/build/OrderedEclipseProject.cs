using System;
using System.Collections.Generic;
using System.Text;

using BuildTask.Flex;

namespace BuildTask.Flex.utils
{
    public class OrderedEclipseProject
    {
        public EclipseFlexProject Project;

        public int Order;

        public OrderedEclipseProject(EclipseFlexProject project, int order)
        {
            this.Project = project;
            this.Order = order;
        }
    }
}
