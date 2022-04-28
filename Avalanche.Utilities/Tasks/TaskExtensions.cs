// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections.Concurrent;

/// <summary>Extension methods for <see cref="Task"/></summary>
public static class TaskExtensions
{
    /// <summary>Read .Result of <paramref name="task"/></summary>
    public static object? Result(this Task task) => TaskReaderMap.Instance.ReadResult(task);
    /// <summary>Read .Result of <paramref name="task"/></summary>
    public static T Result<T>(this Task task) => (T)TaskReaderMap.Instance.ReadResult(task)!;

    /// <summary>Reads .Result property from uncasted <see cref="Task"/></summary>
    class TaskReaderMap
    {
        /// <summary>Singleton</summary>
        static TaskReaderMap instance = new TaskReaderMap();
        /// <summary>Singleton</summary>
        public static TaskReaderMap Instance => instance;

        /// <summary>Task readers.</summary>
        protected ConcurrentDictionary<Type, TaskReader> taskReaders = new();
        /// <summary>Create <see cref="TaskReader{T}"/> for valueType</summary>
        protected Func<Type, TaskReader> createTaskReader;

        /// <summary>Create task reader map</summary>
        public TaskReaderMap() => createTaskReader = CreateTaskReader;

        /// <summary>Read .Result of <paramref name="task"/></summary>
        public object? ReadResult(Task task)
        {
            // Get task's value type
            Type valueType = task.GetType().GetGenericArguments()[0];
            // Get reader
            TaskReader taskReader = taskReaders.GetOrAdd(valueType, createTaskReader);
            // Read
            object? result = taskReader.ReadResult(task);
            // Return
            return result;
        }

        /// <summary>Create <see cref="TaskReader{T}"/> for <paramref name="valueType"/></summary>
        static TaskReader CreateTaskReader(Type valueType) => (TaskReader)Activator.CreateInstance(typeof(TaskReader<>).MakeGenericType(valueType))!;
    }

    /// <summary>Reads .Result property from uncasted <see cref="Task"/></summary>
    abstract class TaskReader
    {
        /// <summary>Read result of <paramref name="task"/></summary>
        public abstract object? ReadResult(Task task);
    }

    /// <summary>Reads .Result property from uncasted <see cref="Task"/></summary>
    class TaskReader<T> : TaskReader
    {
        /// <summary>Read <paramref name="task"/> result.</summary>
        public override object? ReadResult(Task task) => ((Task<T>)task).Result;
    }

}

