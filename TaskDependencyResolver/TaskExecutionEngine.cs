using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskDependencyResolver
{
    public interface ITaskExecutionEngine
    {
        LinkedList<string> ExecuteTasks(ITask currentTask);
    }

    public class TaskExecutionEngine : ITaskExecutionEngine
    {
        private readonly LinkedList<string> _executedTasks = new LinkedList<string>();

        public LinkedList<string> ExecuteTasks(ITask currentTask)
        {
            if (currentTask == null)
            {
                throw new ArgumentNullException();
            }

            if (currentTask.EnumVisitStatus == EnumVisitStatus.Visiting)
            {
                throw new Exception("Circular reference detected");
            }

            if (currentTask.EnumVisitStatus != EnumVisitStatus.Visited)
            {
                currentTask.EnumVisitStatus = EnumVisitStatus.Visiting;

                if (currentTask.DependingTasks.Any())
                {
                    foreach (var dependingTask in currentTask.DependingTasks)
                    {
                        ExecuteTasks(dependingTask);
                    }
                }

                currentTask.ExecuteTask();
                currentTask.EnumVisitStatus = EnumVisitStatus.Visited;
                _executedTasks.AddLast(currentTask.Name);
            }

            return _executedTasks;
        }
    }

}
