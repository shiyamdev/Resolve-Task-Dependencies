using System;
using System.Collections.Generic;

namespace TaskDependencyResolver
{
    public class Task : ITask
    {
        public string Name { get; }

        public Task(string taskName)
        {
            Name = taskName;
        }

        public List<ITask> DependingTasks { get; set; } = new List<ITask>();

        public EnumVisitStatus EnumVisitStatus { get; set; } = EnumVisitStatus.NotVisited;

        public void ExecuteTask()
        {
            Console.WriteLine($"{Name} executed");
        }
    }
}
