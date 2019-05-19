using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TaskDependencyResolver.test
{
    [TestFixture]
    class TaskExecutionEngineTest
    {
        private ITaskExecutionEngine _taskExecutionEngine;
        private ITask _taskA;
        private ITask _taskB;
        private ITask _taskC;
        private ITask _taskD;
        private ITask _taskE;
        private List<ITask> _tasks;


        [SetUp]
        public void SetUp()
        {
            _taskExecutionEngine = new TaskExecutionEngine();

            //Arrange
            // Create instances of task
            _taskA = new Task(nameof(_taskA));
            _taskB = new Task(nameof(_taskB));
            _taskC = new Task(nameof(_taskC));
            _taskD = new Task(nameof(_taskD));
            _taskE = new Task(nameof(_taskE));

            // Setup dependencies
            _taskA.DependingTasks.AddRange(new List<ITask> {_taskB, _taskC});
            _taskB.DependingTasks.AddRange(new List<ITask> {_taskD, _taskE});
            _taskC.DependingTasks.Add(_taskE);

            // Add tasks to the list
            _tasks = new List<ITask> { _taskA, _taskB, _taskC, _taskD, _taskE };
        }

        [Test]
        [TestCase("A", 5)]
        [TestCase("B", 3)]
        [TestCase("C", 2)]
        [TestCase("D", 1)]
        [TestCase("E", 1)]
        public void ExecuteTask_WhenTaskPassedWithDependency_TotalTasksExecuted(string taskName, int expectedResult)
        {
            // Arrange
            var taskToExecute = _tasks.FirstOrDefault(x => x.Name == "_task" + taskName);

            // Act
            var result = _taskExecutionEngine.ExecuteTasks(taskToExecute);

            // Assert
            Assert.That(result.Count, Is.EqualTo(expectedResult));
        }

        [Test]
        public void ExecuteTask_TaskWithNoDependency()
        {
            // Arrange
            var taskToExecute = _tasks.FirstOrDefault(x => x.Name == "_taskD");

            // Act
            var result = _taskExecutionEngine.ExecuteTasks(taskToExecute);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Contains("_taskD"));
        }

        [Test]
        public void ExecuteTask_TaskWithOneDependency()
        {
            // Arrange
            var taskToExecute = _tasks.FirstOrDefault(x => x.Name == "_taskC");

            // Act
            var result = _taskExecutionEngine.ExecuteTasks(taskToExecute);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Contains("_taskC"));
            Assert.That(result.Contains("_taskE"));
            // Check execution order
            Assert.That(result.First.Value, Is.EqualTo("_taskE"));
            //Check main task is executed last
            Assert.That(result.Last.Value, Is.EqualTo("_taskC"));
        }

        [Test]
        public void ExecuteTask_TaskWithTwoDependency()
        {
            // Arrange
            var taskToExecute = _tasks.FirstOrDefault(x => x.Name == "_taskB");

            // Act
            var result = _taskExecutionEngine.ExecuteTasks(taskToExecute);

            // Assert
            // Check Executed items
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Contains("_taskE"));
            Assert.That(result.Contains("_taskD"));
            Assert.That(result.Contains("_taskB"));

            // Check execution order
            Assert.That(result.First.Value, Is.EqualTo("_taskE") | Is.EqualTo("_taskD"));
            //Check main task is executed last
            Assert.That(result.Last.Value, Is.EqualTo("_taskB"));
        }

        [Test]
        public void ExecuteTask_TaskWithMultipleNestedDependency()
        {
            // Arrange
            var taskToExecute = _tasks.FirstOrDefault(x => x.Name == "_taskA");

            // Act
            var result = _taskExecutionEngine.ExecuteTasks(taskToExecute);

            // Assert
            // Check Executed items
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result.Contains("_taskA"));
            Assert.That(result.Contains("_taskB"));
            Assert.That(result.Contains("_taskC"));
            Assert.That(result.Contains("_taskD"));
            Assert.That(result.Contains("_taskE"));

            // Check execution order
            // First executed task is E or D
            Assert.That(result.First.Value, Is.EqualTo("_taskE") | Is.EqualTo("_taskD"));
            // Third executed task is B or C
            Assert.That(result.ToArray()[2], Is.EqualTo("_taskB") | Is.EqualTo("_taskC"));

            // Task E only executed once (even it referenced in 2 tasks)
            Assert.That(result.ToArray().Count(x=>x.Contains("_taskE")), Is.EqualTo(1));

            //Check main task is executed last
            Assert.That(result.Last.Value, Is.EqualTo("_taskA"));
        }

        [Test]
        public void ExecuteTask_WhenEmptyTaskPassed_ThrowArgumentNullException()
        {
            // Assert
            Assert.That(() => _taskExecutionEngine.ExecuteTasks(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ExecuteTask_WhenTaskWithCircularReferencePassed_DetectAndThrowException()
        {
            // Arrange
            // Create instances of task
            ITask taskA = new Task(nameof(taskA));
            ITask taskB = new Task(nameof(taskB));
            ITask taskC = new Task(nameof(taskC));

            // Setup dependencies
            taskA.DependingTasks.Add(taskB);
            taskB.DependingTasks.Add(taskC);
            taskC.DependingTasks.Add(taskA);
            
            // Assert
            Assert.That(()=> _taskExecutionEngine.ExecuteTasks(taskA), Throws.InstanceOf<Exception>().With.Message.EqualTo("Circular reference detected").IgnoreCase);
        }
    }
}
