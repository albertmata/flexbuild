using System;
using System.Collections.Generic;
using System.Text;

namespace BuildTask.Flex.utils
{
    public class ProjectOrderBuilder
    {
        //Se supone que no puede haber dependencias cíclicas
        public static EclipseFlexProject[] BuildProjectOrder(EclipseFlexProject mainProject)
        {
            Dictionary<EclipseFlexProject, int> list = new Dictionary<EclipseFlexProject, int>();
            
            int treeDepth = 0;
            list.Add(mainProject, treeDepth);

            FollowDependencies(mainProject, list, treeDepth + 1);

            //Aqui tenemos el arbol de dependencias, ahora creamos un array con tuplas de proyecto - profundidad y ordenamos de mayor a menor profundidad

            OrderedEclipseProject[] orderArray = new OrderedEclipseProject[list.Count];
            int i=0;
            foreach (EclipseFlexProject project in list.Keys)
            {
                orderArray[i++] = new OrderedEclipseProject(project, list[project]);
            }
            Array.Sort(orderArray, new Comparison<OrderedEclipseProject>(ProjectOrderBuilder.CompareOrderedEclipseProject));

            EclipseFlexProject[] returnArray = new EclipseFlexProject[orderArray.Length];
            for (i = 0; i < orderArray.Length; i++)
            {
                returnArray[i] = orderArray[i].Project;
            }

            return returnArray;
        }

        private static void FollowDependencies(EclipseFlexProject parent, Dictionary<EclipseFlexProject, int> list, int treeDepth)
        {
            foreach(EclipseFlexProject dependency in parent.Dependencies)
            {
                if (list.ContainsKey(dependency))
                {
                    //Actualizamos profundidad
                    list[dependency] = Math.Max(treeDepth, list[dependency]);
                }
                else
                {
                    list.Add(dependency, treeDepth);
                }
                if (dependency.Dependencies.Count > 0)
                {
                    FollowDependencies(dependency, list, treeDepth + 1);
                }
            }
        }

        private static int CompareOrderedEclipseProject(OrderedEclipseProject x, OrderedEclipseProject y)
        {
            //Ordernamos al revés
            return y.Order.CompareTo(x.Order);
        }
    }
}
