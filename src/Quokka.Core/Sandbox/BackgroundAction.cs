using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quokka.Diagnostics;

namespace Quokka.Sandbox
{
	/// <summary>
	/// 	Called 'ThreadOp' as a working name, out of nostalgia for similar
	/// 	concept developed in C++ in late 20th Century
	/// </summary>
	public class BackgroundAction
	{
		public Action DoWork { get; set; }
		public Action WhenComplete { get; set; }
		public Action<Exception> WhenError { get; set; }
		public SynchronizationContext SynchronizationContext { get; set; }
		public IList<IBackgroundActionModule> Modules { get; private set; }

		public BackgroundAction()
		{
			Modules = new List<IBackgroundActionModule>();
		}

		public virtual void Run()
		{
			try
			{
				RunDoWorkAction();
				RunCompletionAction();
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					throw;
				}
				RunErrorAction(ex);
			}
			finally
			{
				_actionContext = null;
			}
		}

		public static IBackgroundActionDoWorkBuilder Define()
		{
			return new Builder();
		}

		private class Builder : IBackgroundActionDoWorkBuilder,
		                        IBackgroundActionCompletionBuilder,
		                        IBackgroundActionOptionsBuilder
		{
			public readonly BackgroundAction Action;

			public Builder()
			{
				Action = new BackgroundAction();
			}

			public IBackgroundActionCompletionBuilder DoWork(Action action)
			{
				Action.DoWork = action;
				return this;
			}

			public IBackgroundActionCompletionBuilder<TResult> DoWork<TResult>(Func<TResult> action)
			{
				var actionWithResult = new FunctionStoringResult<TResult>(action);
				Action.DoWork = actionWithResult.Invoke;
				return new Builder<TResult>(this);
			}

			public IBackgroundActionOptionsBuilder WhenComplete(Action action)
			{
				Action.WhenComplete = action;
				return this;
			}

			public IBackgroundActionOptionsBuilder WhenError(Action<Exception> action)
			{
				Action.WhenError = action;
				return this;
			}

			public IBackgroundActionOptionsBuilder WhenCancelled(Action action)
			{
				throw new NotImplementedException();
			}

			public IBackgroundActionOptionsBuilder AddModule(IBackgroundActionModule module)
			{
				if (module != null)
				{
					Action.Modules.Add(module);
				}
				return this;
			}

			public BackgroundAction Create()
			{
				return Action;
			}
		}

		private class Builder<TResult> : Builder, IBackgroundActionCompletionBuilder<TResult>
		{
			private readonly Builder _builder;

			public Builder(Builder builder)
			{
				_builder = Verify.ArgumentNotNull(builder, "builder");
			}

			public IBackgroundActionOptionsBuilder WhenComplete(Action<TResult> action)
			{
				var actionAcceptingResult = new ActionAcceptingResult<TResult>(action);
				_builder.Action.WhenComplete = actionAcceptingResult.Invoke;
				return _builder;
			}
		}

		private class FunctionStoringResult<TResult>
		{
			private readonly Func<TResult> _func;

			public FunctionStoringResult(Func<TResult> func)
			{
				_func = Verify.ArgumentNotNull(func, "func");
			}

			public void Invoke()
			{
				if (_actionContext == null)
				{
					_actionContext = new ActionContext();
				}
				_actionContext.Result = _func();
			}
		}

		private class ActionAcceptingResult<TResult>
		{
			private readonly Action<TResult> _action;

			public ActionAcceptingResult(Action<TResult> action)
			{
				_action = Verify.ArgumentNotNull(action, "action");
			}

			public void Invoke()
			{
				TResult result = (TResult) _actionContext.Result;
				_action(result);
			}
		}

		private class ActionContext
		{
			public List<IBackgroundActionModule> Modules;
			public object Result;
		}

		[ThreadStatic] private static ActionContext _actionContext;

		private void RunBeforeActions()
		{
			foreach (var module in Modules)
			{
				module.Before();

				// Module has been successfully run. Remember it so that the cleanup
				// methods can be run.
				if (_actionContext == null)
				{
					_actionContext = new ActionContext();
				}
				if (_actionContext.Modules == null)
				{
					_actionContext.Modules = new List<IBackgroundActionModule>();
				}
				_actionContext.Modules.Add(module);
			}
		}

		private static void RunAfterActions()
		{
			if (_actionContext == null || _actionContext.Modules == null)
			{
				return;
			}

			List<Exception> exceptions = null;

			for (int index = _actionContext.Modules.Count - 1; index >= 0; --index)
			{
				var module = _actionContext.Modules[index];
				try
				{
					module.After();
				}
				catch (Exception ex)
				{
					if (exceptions == null)
					{
						exceptions = new List<Exception>();
					}
					exceptions.Add(ex);
				}
			}

			// TODO: create an exception that groups all of the exceptions as
			// properties of the outer exception. For now, just throw the first exception
			if (exceptions != null)
			{
				throw exceptions.First();
			}
		}

		private static void RunErrorActions(Exception exception)
		{
			if (_actionContext == null || _actionContext.Modules == null)
			{
				return;
			}

			List<Exception> exceptions = null;

			for (int index = _actionContext.Modules.Count - 1; index >= 0; --index)
			{
				var module = _actionContext.Modules[index];
				try
				{
					module.Error(exception);
				}
				catch (OutOfMemoryException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (exceptions == null)
					{
						exceptions = new List<Exception>();
					}
					exceptions.Add(ex);
				}
			}

			// TODO: need to know what to do with these exceptions
			// properties of the outer exception. For now, just throw the first exception
			if (exceptions != null)
			{
				// TODO:
			}
		}

		private static void RunFinishedActions()
		{
			if (_actionContext == null || _actionContext.Modules == null)
			{
				return;
			}

			List<Exception> exceptions = null;

			for (int index = _actionContext.Modules.Count - 1; index >= 0; --index)
			{
				var module = _actionContext.Modules[index];
				try
				{
					module.Finished();
				}
				catch (OutOfMemoryException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (exceptions == null)
					{
						exceptions = new List<Exception>();
					}
					exceptions.Add(ex);
				}
			}

			// TODO: create an exception that groups all of the exceptions as
			// properties of the outer exception. For now, just throw the first exception
			if (exceptions != null)
			{
				throw exceptions.First();
			}
		}

		private void RunDoWorkAction()
		{
			try
			{
				RunBeforeActions();
				if (DoWork != null)
				{
					DoWork();
				}
				RunAfterActions();
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					RunErrorActions(ex);
				}
				throw;
			}
			finally
			{
				RunFinishedActions();
			}
		}

		private void RunCompletionAction()
		{
			if (WhenComplete != null)
			{
				RunSynchronized(WhenComplete);
			}
		}

		private void RunErrorAction(Exception ex)
		{
			if (WhenError != null)
			{
				RunSynchronized(() => WhenError(ex));
			}
		}

		private void RunSynchronized(Action action)
		{
			if (action == null)
			{
				return;
			}

			if (SynchronizationContext == null)
			{
				action();
			}
			else
			{
				SynchronizationContext.Send(delegate { action(); }, null);
			}
		}
	}

	public interface IBackgroundActionDoWorkBuilder
	{
		IBackgroundActionCompletionBuilder DoWork(Action action);
		IBackgroundActionCompletionBuilder<TResult> DoWork<TResult>(Func<TResult> action);
	}

	public interface IBackgroundActionCompletionBuilder
	{
		IBackgroundActionOptionsBuilder WhenComplete(Action action);
	}

	public interface IBackgroundActionCompletionBuilder<TResult>
	{
		IBackgroundActionOptionsBuilder WhenComplete(Action<TResult> action);
	}

	public interface IBackgroundActionOptionsBuilder
	{
		IBackgroundActionOptionsBuilder WhenError(Action<Exception> action);
		IBackgroundActionOptionsBuilder WhenCancelled(Action action);
		IBackgroundActionOptionsBuilder AddModule(IBackgroundActionModule module);
		BackgroundAction Create();
	}

	public interface IBackgroundActionModule
	{
		void Before();
		void After();
		void Error(Exception ex);
		void Finished();
	}

	public class ThreadOp<TResult>
	{
		public ThreadOp(Func<TResult> func)
		{
		}
	}

	public static class BackgroundActionTransactionExtensions
	{
		public static IBackgroundActionOptionsBuilder WithTransaction(this IBackgroundActionOptionsBuilder builder)
		{
			// Add a module here
			builder.AddModule(null);
			return builder;
		}

		public static IBackgroundActionOptionsBuilder WithInitialDelayOf(this IBackgroundActionOptionsBuilder builder,
		                                                                 int msec)
		{
			return builder;
		}
	}

	public class Example
	{
		public void DoSomething()
		{
			var threadop = BackgroundAction.Define()
				.DoWork(() => DoWork(1, 2))
				.WhenComplete(WhenFinished)
				.WithTransaction()
				.WithInitialDelayOf(1000)
				.Create();

			threadop.Run();
		}

		private string DoWork(int n1, int n2)
		{
			return (n1 + n2).ToString();
		}

		private void WhenFinished(string result)
		{
		}
	}
}