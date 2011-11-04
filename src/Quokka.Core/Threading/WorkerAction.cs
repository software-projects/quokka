using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quokka.Diagnostics;

namespace Quokka.Threading
{
	public class WorkerAction
	{
		[ThreadStatic] private static ActionContext _actionContext;

		public Action DoWork { get; set; }
		public Action WhenComplete { get; set; }
		public Action<Exception> WhenError { get; set; }
		public Action WhenCanceled { get; set; }
		public SynchronizationContext SynchronizationContext { get; set; }
		public IList<IWorkerActionModule> Modules { get; private set; }

		public WorkerAction()
		{
			Modules = new List<IWorkerActionModule>();
		}

		public virtual void Run()
		{
			// To handle recursive calls, keep a copy of the previous
			// action context on the stack.
			var previousActionContext = _actionContext;

			try
			{
				_actionContext = new ActionContext();
				RunDoWorkAction();
				RunCompletionAction();
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					throw;
				}
				if (ex is OperationCanceledException && WhenCanceled != null)
				{
					RunWhenCanceled();
				}
				else
				{
					RunErrorAction(ex);
				}
			}
			finally
			{
				_actionContext = previousActionContext;
			}
		}

		public static IDoWorkBuilder Define()
		{
			return new Builder();
		}

		#region Builder interfaces

		public interface IDoWorkBuilder
		{
			ICompletionBuilder DoWork(Action action);
			ICompletionBuilder<TResult> DoWork<TResult>(Func<TResult> action);
		}

		public interface ICompletionBuilder : IOptionsBuilder
		{
			IOptionsBuilder WhenComplete(Action action);
		}

		public interface ICompletionBuilder<TResult> : IOptionsBuilder
		{
			IOptionsBuilder WhenComplete(Action<TResult> action);
		}

		public interface IOptionsBuilder
		{
			IOptionsBuilder WhenError(Action<Exception> action);
			IOptionsBuilder WhenCanceled(Action action);
			IOptionsBuilder AddModule(IWorkerActionModule module);
			WorkerAction Create();
			WorkerAction Run(Worker worker);
		}

		#endregion

		#region Builder classes

		private class Builder : IDoWorkBuilder, ICompletionBuilder
		{
			public readonly WorkerAction Action;

			public Builder()
			{
				Action = new WorkerAction();
			}

			protected Builder(WorkerAction action)
			{
				Action = Verify.ArgumentNotNull(action, "action");
			}

			public ICompletionBuilder DoWork(Action action)
			{
				Action.DoWork = action;
				return this;
			}

			public ICompletionBuilder<TResult> DoWork<TResult>(Func<TResult> action)
			{
				var actionWithResult = new FunctionStoringResult<TResult>(action);
				Action.DoWork = actionWithResult.Invoke;
				return new Builder<TResult>(this);
			}

			public IOptionsBuilder WhenComplete(Action action)
			{
				Action.WhenComplete = action;
				return this;
			}

			public IOptionsBuilder WhenError(Action<Exception> action)
			{
				Action.WhenError = action;
				return this;
			}

			public IOptionsBuilder WhenCanceled(Action action)
			{
				Action.WhenCanceled = action;
				return this;
			}

			public IOptionsBuilder AddModule(IWorkerActionModule module)
			{
				if (module != null)
				{
					Action.Modules.Add(module);
				}
				return this;
			}

			public WorkerAction Create()
			{
				return Action;
			}

			public WorkerAction Run(Worker worker)
			{
				worker.Run(Action);
				return Action;
			}
		}

		private class Builder<TResult> : Builder, ICompletionBuilder<TResult>
		{
			private readonly Builder _builder;

			public Builder(Builder builder) : base(builder.Action)
			{
				_builder = builder;
			}

			public IOptionsBuilder WhenComplete(Action<TResult> action)
			{
				var actionAcceptingResult = new ActionAcceptingResult<TResult>(action);
				Action.WhenComplete = actionAcceptingResult.Invoke;
				return _builder;
			}
		}

		#endregion

		#region Action classes

		private class ActionContext
		{
			public List<IWorkerActionModule> Modules;
			public object Result;
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

		#endregion

		#region Action methods

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
				if (!ex.IsCorruptedStateException())
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

		private void RunWhenCanceled()
		{
			if (WhenCanceled != null)
			{
				RunSynchronized(WhenCanceled);
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
				// No SynchronizationContext, so just run on the current thread.
				action();
			}
			else
			{
				// SynchronizationContext exists, so run in that SC. Because the SC
				// is likely to mean running on another thread, we have to be careful
				// to transfer the action context over to that thread.

				// Take a copy of the action context into a local variable
				var context = _actionContext;
				try
				{
					SynchronizationContext.Send(delegate
					                            	{
														// Set the thread-static _actionContext variable
														// for the current thread.
					                            		_actionContext = context;
					                            		try
					                            		{
															// Perform the action
					                            			action();
					                            		}
					                            		finally
					                            		{
															// Remove the action context from the worker thread.
					                            			_actionContext = null;
					                            		}
					                            	}, null);
				}
				finally
				{
					// Even though it is highly probable that the SynchronizationContext meant that
					// the action was performed on another thread, it *is* possible that the action
					// was performed on the current thread. 
					//
					// For this reason, be sure to restore the action context for this thread, because
					// it was cleared out in the Send delegate.
					_actionContext = context;
				}
			}
		}

		private void RunBeforeActions()
		{
			foreach (var module in Modules)
			{
				module.Before();

				// Module has been successfully run. Remember it so that the cleanup
				// methods can be run.
				if (_actionContext.Modules == null)
				{
					_actionContext.Modules = new List<IWorkerActionModule>();
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
					if (exceptions == null)
					{
						module.After();
					}
					else
					{
						module.Error(exceptions.First());
					}
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

			// TODO: what to do with these exceptions
			if (exceptions != null)
			{
				//throw exceptions.First();
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
				catch (Exception ex)
				{
					if (ex.IsCorruptedStateException())
					{
						throw;
					}
					if (exceptions == null)
					{
						exceptions = new List<Exception>();
					}
					exceptions.Add(ex);
				}
			}

			// TODO: need to know what to do with these exceptions
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
				catch (Exception ex)
				{
					if (ex.IsCorruptedStateException())
					{
						throw;
					}
					if (exceptions == null)
					{
						exceptions = new List<Exception>();
					}
					exceptions.Add(ex);
				}
			}

			// TODO: create an exception that groups all of the exceptions as
			if (exceptions != null)
			{
			}
		}

		#endregion
	}
}