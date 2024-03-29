using System;
using System.Collections.Generic;
using System.Text;

using BuildTask.Flex;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
       {
            FlexBuild flex = new FlexBuild();
            flex.BuildEngine = new MockBuild();
            flex.WorkSpacePath = @"D:\@Work\Work2005\FlexProjects\WebCounterWksp";
            flex.ProjectsBasePath = @"D:\@Work\Work2005";
            flex.NewBasePath = @"C:\dd";
            flex.ReplaceProjectPaths = true;
            flex.Execute();
            Console.WriteLine("Task executed");
            Console.ReadLine();
        }
    }
}
