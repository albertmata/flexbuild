using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex
{
    public abstract class FlexPropertiesBase
    {
        public virtual string[] IncludeClasses
        {
            get{ return new string[0]; }
        }        

        public virtual ProjectResource[] IncludeResources
        {
            get { return new ProjectResource[0]; }
        }
    }
}
