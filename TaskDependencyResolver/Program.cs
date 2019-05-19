using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskDependencyResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var taskExecutionEngine = new TaskExecutionEngine();
            var executedTasksQueue =  taskExecutionEngine.ExecuteTasks(ChainDependencyAndGetTask("A"));

            ////Circular dependency test
            //var executedTasksQueue =  taskExecutionEngine.ExecuteTasks(CircularDependencyTest());

            Console.WriteLine("---- Executed Task ----");

            foreach (var executedTask in executedTasksQueue)
            {
                Console.WriteLine(executedTask);
            }

            Console.ReadLine();
        }


        private static ITask ChainDependencyAndGetTask(string taskName)
        {
            // Create instances of task
            ITask taskA = new Task(nameof(taskA));
            ITask taskB = new Task(nameof(taskB));
            ITask taskC = new Task(nameof(taskC));
            ITask taskD = new Task(nameof(taskD));
            ITask taskE = new Task(nameof(taskE));

            // Setup dependencies
            taskA.DependingTasks.AddRange(new List<ITask> { taskB, taskC });
            taskB.DependingTasks.AddRange(new List<ITask> { taskD, taskE });
            taskC.DependingTasks.Add(taskE);

            // Add tasks to the list
            var tasks = new List<ITask>{taskA, taskB, taskC, taskD, taskE};

            return tasks.FirstOrDefault(x => x.Name == "task" + taskName);
        }

        private static ITask CircularDependencyTest()
        {
            // Create instances of task
            ITask taskA = new Task(nameof(taskA));
            ITask taskB = new Task(nameof(taskB));
            ITask taskC = new Task(nameof(taskC));

            // Setup dependencies
            taskA.DependingTasks.Add(taskB);
            taskB.DependingTasks.Add(taskC);
            taskC.DependingTasks.Add(taskA);

            return taskA;
        }
    }
}
