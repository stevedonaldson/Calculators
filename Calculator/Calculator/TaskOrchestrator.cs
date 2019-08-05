using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculator
{
	public class Dependency
	{
		public string TaskName { get; set; }
		public Task<object> WaitOn { get; set; }
	}

	public interface ITaskInfo
	{
		string TaskName { get; set; }
		TaskCompletionSource<object> TaskCompletion { get; set; }
		ICollection<Dependency> Dependencies { get; set; }
		object Response { get; set; }
		Task ExecuteJob();
	}

	public class TaskInfo<TRequest, TResponse> : ITaskInfo where TResponse : class, new()
	{
		public string TaskName { get; set; }
		public TaskCompletionSource<object> TaskCompletion { get; set; }
		public ICollection<Dependency> Dependencies { get; set; }
		public Func<object[], TRequest> Initialize { get; set; }
		public object Response { get; set; }

		public async Task ExecuteJob()
		{
			TRequest request;

			if (Dependencies != null && Dependencies.Count > 0)
			{
				var tasks = Dependencies.Select(y => y.WaitOn).ToArray();
				var responses = await Task.WhenAll(tasks);
				request = Initialize(responses);
			}
			else
			{
				request = Initialize(null);
			}

			var med = new Mediator();
			Response = await med.Send<TRequest, TResponse>(request);
			TaskCompletion.SetResult(Response);
		}
	}

	public class TaskOrchestrator
	{
		private Dictionary<string, ITaskInfo> taskTable = new Dictionary<string, ITaskInfo>();

		public TaskOrchestrator()
		{
			var info = new TaskInfo<Request1, Response1>
			{
				TaskName = "Request1",
				TaskCompletion = new TaskCompletionSource<object>(),
				Dependencies = null,
				Initialize = (obj) => new Request1() { ReqProp1 = "Test1" }
			};

			var info2 = new TaskInfo<Request2, Response2>
			{
				TaskName = "Request2",
				TaskCompletion = new TaskCompletionSource<object>(),
				//Dependencies = new List<Dependency>
				//{
				//	new Dependency
				//	{
				//		TaskName = "Request1"
				//	}
				//},
				//Initialize = (obj) => new Request2() { ReqProp2 = ((Response1)obj[0]).ResProp1 }
				Dependencies = null,
				Initialize = (obj) => new Request2() { ReqProp2 = "Test1" }
			};

			taskTable.Add(info.TaskName, info);
			taskTable.Add(info2.TaskName, info2);

			taskTable.Values
				.Where(y => y.Dependencies != null)
				.SelectMany(y => y.Dependencies)
				.ToList()
				.ForEach(y => y.WaitOn = taskTable[y.TaskName].TaskCompletion.Task);
		}

		public async Task<bool> Launch()
		{
			taskTable.Values
				.ToList()
				.ForEach(y => y.ExecuteJob());

			var allTasks = taskTable.Values
				.Select(y => y.TaskCompletion.Task)
				.ToArray();

			await Task.WhenAll(allTasks);
			return true;
		}

		//public void TaskCompletedEvent(string taskName, object state)
		//{
		//	var newTasksToLaunch = new List<ITaskInfo>();
		//	lock (_lock)
		//	{
		//		taskTable[taskName].IsCompleted = true;

		//		taskTable.Values
		//			.Where(y => y.TaskName != taskName && y.Dependencies != null)
		//			.Where(y => y.Dependencies.Contains(taskName))
		//			.ToList()
		//			.ForEach(y =>
		//			{
		//				if (y.Dependencies.Select(z => taskTable[z]).All(w => w.IsCompleted))
		//					newTasksToLaunch.Add(y);
		//			});
		//	}

		//	newTasksToLaunch.ForEach(y => y.ExecuteJob(state));
		//}
	}






	public class Mediator
	{
		public Mediator()
		{
		}
		public async Task<K> Send<T, K>(T request) where K : class, new()
		{
			var tt = Task.Delay(1000);
			await tt;
			return new K();
		}
	}

	public class Request1
	{
		public string ReqProp1 { get; set; }
	}

	public class Response1
	{
		public string ResProp1 { get; set; }
	}

	public class Request2
	{
		public string ReqProp2 { get; set; }
	}

	public class Response2
	{
		public string ResProp2 { get; set; }
	}

	public class Request3
	{
		public string ReqProp3 { get; set; }
	}

	public class Response3
	{
		public string ResProp3 { get; set; }
	}
}