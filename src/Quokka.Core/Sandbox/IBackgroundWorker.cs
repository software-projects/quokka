using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Quokka.ServiceLocation;

namespace Quokka.Sandbox
{
	public interface IBackWorker
	{
		bool IsBusy { get; }
		bool CanCancel { get; }
		IBackWorkerCompletionBuilder<TResult> DoWork<TResult>(Func<TResult> func);
		IBackWorkerCompletionBuilder DoWork(Action func);
	}

	public static class BackWorker
	{
		// is this a good idea? static class containing various things that can be done
		public static IBackWorker Current { get; private set; }

	}

	public interface IBackWorkerCompletionBuilder
	{
		IBackWorkerCompletionBuilder WhenComplete(Action action);
	}

	public interface IBackWorkerCompletionBuilder<TResult>
	{
		IBackWorkerBuilder WhenComplete(Action<TResult> action);
	}

	public interface IBackWorkerBuilder
	{
		IBackWorkerBuilder WhenError(Action<Exception> action);
		IBackWorkerBuilder WhenCancelled(Action action);
		IBackWorkerBuilder AddModule(IBackWorkerModule module);
	}

	public interface IBackWorkerModule
	{
		void Before();
		void After();
		void Error(Exception ex);
	}

	namespace DosEdge.Core.UIWorker
	{
		/// <summary>
		/// Fluent background worker.
		///
		/// This class is a wrapper around BackgroundWorker that uses a fluent configuration.
		/// The DoWork method takes lambda expression of the following syntax:
		///
		/// ()=>DoWork()
		///        .WhenComplete(Action);
		///
		/// eventArgs=>DoWork(eventArgs)
		///        .WhenComplete(Action);
		///
		/// Where eventArgs is a DoWorkEventArgs
		///
		/// The Action specified in WhenComplete takes an argument with the same type as the
		/// return value from your DoWork method with an Optional RunWorkerCompletedEventArgs
		///
		/// _worker
		///        .DoWork(()=>AddNumbers(1,2))
		///        .WhenComplete(PrintResult);
		///
		///
		/// private int AddNumbers(int a, int b)
		/// {
		///        return a + b;
		/// }
		///
		/// private void PrintResult(int result, RunWorkerEventArgs)
		/// {
		///        Console.WriteLine(result)
		/// }
		///
		/// </summary>
		public interface IFluentUIWorker
		{
			IFluentUIWorkerBuilder<TResult> DoWork<TResult>(Expression<Func<TResult>> expression);
			IFluentUIWorkerBuilder<TResult> DoWork<TResult>(Expression<Func<DoWorkEventArgs, TResult>> expression);
			IFluentUIWorkerBuilder
   DoWork(Expression<Action<DoWorkEventArgs>> expression);
			IFluentUIWorkerBuilder DoWork(Expression<Action> expression);

			/// <summary>
			/// Run the background job asynchronously.
			/// </summary>
			void RunAsync();

			/// <summary>
			/// Cancel the background worker.
			/// </summary>
			void CancelAsync();

			/// <summary>
			/// Is a cancellation request pending.
			/// </summary>
			bool CancellationPending();
		}


		/// <summary>
		/// FluentUIWorkerBuilder is returned by DoWork and is used to specify the
		/// work complete action. It uses the return type of the DoWork function to
		/// determine the parameter for the WhenComplete function.
		/// </summary>
		public interface IFluentUIWorkerBuilder<TResult>
		{
			void WhenComplete(Action<TResult, RunWorkerCompletedEventArgs> action);
			void WhenComplete(Action<TResult> action);
		}

		/// <summary>
		/// This builder is for DoWork functions wihout a return value.
		/// </summary>
		public interface IFluentUIWorkerBuilder
		{
			void WhenComplete(Action<RunWorkerCompletedEventArgs> action);
			void WhenComplete(Action saction);
		}

		/// <summary>
		/// See IFluentUIWorkerBuilder
		/// </summary>
		public class FluentUIWorkerBuilder : IFluentUIWorkerBuilder
		{
			private readonly FluentUIWorker _uiWorker;
			public FluentUIWorkerBuilder(FluentUIWorker uiWorker)
			{
				_uiWorker = uiWorker;
			}
			public void WhenComplete(Action<RunWorkerCompletedEventArgs>
   action)
			{
				_uiWorker.WorkCompleteFunction =
					 (obj, eventArgs) => action.Invoke(eventArgs);
			}

			public void WhenComplete(Action action)
			{
				_uiWorker.WorkCompleteFunction =
					(obj, eventArgs) => action.Invoke();
			}
		}

		/// <summary>
		/// See IFluentUIWorkerBuilder
		/// </summary>
		public class FluentUIWorkerBuilder<TResult> :
   IFluentUIWorkerBuilder<TResult>
		{
			private readonly FluentUIWorker _uiWorker;

			public FluentUIWorkerBuilder(FluentUIWorker uiWorker)
			{
				_uiWorker = uiWorker;
			}

			public void WhenComplete(Action<TResult,
   RunWorkerCompletedEventArgs> action)
			{
				_uiWorker.WorkCompleteFunction =
					(obj, eventArgs) => action.Invoke((TResult)obj, eventArgs);
			}

			public void WhenComplete(Action<TResult> action)
			{
				_uiWorker.WorkCompleteFunction =
					(obj, eventArgs) => action.Invoke((TResult)obj);
			}
		}

		public interface IAOPAttribute
		{
			void Before();
			void After();
		}


		/// <summary>
		/// See IFluentUIWorker
		/// </summary>
		[PerRequest(typeof(IFluentUIWorker))]
		public class FluentUIWorker : IFluentUIWorker
		{
			protected readonly BackgroundWorker Worker;
			protected readonly IList<IAOPAttribute> AOPAttributes;

			public Func<DoWorkEventArgs, object> WorkFunction;
			public Action<object, RunWorkerCompletedEventArgs> WorkCompleteFunction;

			public bool IsBusy { get { return Worker.IsBusy; } }

			public FluentUIWorker()
			{
				Worker = new BackgroundWorker { WorkerSupportsCancellation = true };
				Worker.DoWork += WorkerDoWork;
				Worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
				AOPAttributes = new List<IAOPAttribute>();
			}

			public void CancelAsync()
			{
				Worker.CancelAsync();
			}

			public bool CancellationPending()
			{
				return Worker.CancellationPending;
			}

			protected void WorkerDoWork(object sender, DoWorkEventArgs e)
			{
				if (WorkFunction != null)
				{
					try
					{
						RunAOPBefore();
						var result = WorkFunction.Invoke(e);
						e.Result = result;
					}
					finally
					{
						RunAOPAfter();
					}
				}
			}

			void RunAOPBefore()
			{
				foreach (var aopAttribute in AOPAttributes)
				{
					aopAttribute.Before();
				}
			}

			private void RunAOPAfter()
			{
				foreach (var aopAttribute in AOPAttributes.Reverse())
				{
					aopAttribute.After();
				}
			}

			protected void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
			{
				if (WorkCompleteFunction != null)
				{
					object arg = null;
					if (e.Error == null)
					{
						arg = e.Result;
					}

					WorkCompleteFunction.Invoke(arg, e);
				}
			}

			public IFluentUIWorkerBuilder<TResult> DoWork<TResult>(Expression<Func<TResult>> expression)
			{
				ParseAttributes(expression);
				WorkFunction = eventArgs => expression.Compile().Invoke();
				return new FluentUIWorkerBuilder<TResult>(this);
			}

			public IFluentUIWorkerBuilder<TResult> DoWork<TResult>(Expression<Func<DoWorkEventArgs, TResult>> expression)
			{
				ParseAttributes(expression);
				WorkFunction = eventArgs => expression.Compile().Invoke(eventArgs);
				return new FluentUIWorkerBuilder<TResult>(this);
			}

			public IFluentUIWorkerBuilder DoWork(Expression<Action> expression)
			{
				ParseAttributes(expression);
				WorkFunction = eventArgs =>
				{
					expression.Compile().Invoke();
					return null;
				};
				return new FluentUIWorkerBuilder(this);
			}

			public IFluentUIWorkerBuilder
   DoWork(Expression<Action<DoWorkEventArgs>> expression)
			{
				ParseAttributes(expression);
				WorkFunction = eventArgs =>
				{
					expression.Compile().Invoke(eventArgs);
					return null;
				};
				return new FluentUIWorkerBuilder(this);
			}

			private void ParseAttributes<T>(T expression)
				where T : Expression
			{
				AOPAttributes.Clear();

				var lambda = expression as LambdaExpression;
				if (lambda == null)
					return;

				var body = lambda.Body as MethodCallExpression;
				if (body == null)
					return;

				var attributes =
   body.Method.GetCustomAttributes(typeof(IAOPAttribute), true);
				foreach (var attribute in attributes)
				{
					AOPAttributes.Add((IAOPAttribute)attribute);
				}
			}

			public virtual void RunAsync()
			{
				Worker.RunWorkerAsync();
			}
		}
	}
	class IBackgroundWorker
	{
	}
}
