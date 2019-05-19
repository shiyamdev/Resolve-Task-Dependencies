using System.Collections.Generic;

namespace TaskDependencyResolver
{
    public interface ITask
    {
        string Name { get; }

        List<ITask> DependingTasks { get; set; }
        
        EnumVisitStatus EnumVisitStatus { get; set; }
        
        void ExecuteTask();
    }
}
