using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

namespace Quokka.Sandbox
{
	// This could be implemented by SqlQuery<T>
	public interface IListQuery<T>
	{
		IList<T> ExecuteList();
	}

	public interface IBus
	{
		// Queues a query to run locally
		IBusQueryHandler<T> Query<T>(IListQuery<T> query);

		// Sends a request to be handled either locally or remotely
		IBusResponseHandler Send<T>(T message);

		void Reply<T>(T message);
		void Publish<T>(T message);
	}

	public interface IBusErrorHandler
	{
		void HandleError(Action<ErrorReport> errorHandler);
	}

	public interface IBusResponseHandler : IBusErrorHandler
	{
		IBusResponseHandler HandleResponse<T>(Action<T> responseHandler);
	}

	public interface IBusQueryHandler<T>
	{
		/// <summary>
		/// Handle the result when a list is expected
		/// </summary>
		IBusErrorHandler HandleResult(Action<IList<T>> result);

		/// <summary>
		/// Handle the query result when only one item is expected
		/// </summary>
		IBusErrorHandler HandleResult(Action<T> result);
	}

	public interface IReplyChannel
	{
		void Reply<T>(T message) where T : class;
	}

	public class MessageBundle
	{
		public IReplyChannel ReplyChannel { get; set; }
		public object[] Messages { get; set; }
	}

	public interface IMessageModule
	{
		void Init();
		void Stop();

		void MessageArrived(MessageBundle messageBundle);
		void MessageProcessingStarting(MessageBundle messageBundle, ICollection<object> messageHandlers);
		void MessageProcessingSucceeded(MessageBundle messageBundle);
		void MessageProcessingFailure(MessageBundle messageBundle, object message, Exception ex);
		void MessageProcessingFinished(MessageBundle messageBundle);
	}

	public class CurrentMessageInformation
	{
		
	}

	public interface IMessageHandler<T>
	{
		void Handle(T message);
	}

	public class MessageProcessor
	{
		public IServiceLocator ServiceLocator { get; set; }

		private List<IMessageModule> _messageModules;

		public void Init()
		{
			_messageModules = new List<IMessageModule>(ServiceLocator.GetAllInstances<IMessageModule>());
		}

		void ProcessMessage(MessageBundle messageBundle)
		{
			object currentMessage = null;
			try
			{
				if (messageBundle.Messages == null || messageBundle.Messages.Length == 0)
				{
					// Nothing to do if there are no messages
					return;
				}

				foreach (var module in _messageModules)
				{
					module.MessageArrived(messageBundle);
				}

				var allHandlers = new List<object>();
				var handlersPerMessage = new Queue<List<object>>();

				foreach (var message in messageBundle.Messages)
				{
					var genericTypeDefinition = typeof (IMessageHandler<>);
					var genericType = genericTypeDefinition.MakeGenericType(message.GetType());
					var handlersForThisMessage = new List<object>();
					handlersPerMessage.Enqueue(handlersForThisMessage);
					foreach (var messageHandler in ServiceLocator.GetAllInstances(genericType))
					{
						allHandlers.Add(messageHandler);
						handlersForThisMessage.Add(messageHandler);
					}
				}

				foreach (var module in _messageModules)
				{
					module.MessageProcessingStarting(messageBundle, allHandlers);
				}

				foreach (var message in messageBundle.Messages)
				{
					currentMessage = message;
					var handlers = handlersPerMessage.Dequeue();
					object[] parameters = new[] { message };
					foreach (var handler in handlers)
					{
						var method = handler.GetType().GetMethod("Handle");
						method.Invoke(handler, parameters);
					}
				}

				currentMessage = null;

				foreach (var module in _messageModules)
				{
					module.MessageProcessingSucceeded(messageBundle);
				}
			}
			catch (Exception ex)
			{
				foreach (var module in _messageModules)
				{
					module.MessageProcessingFailure(messageBundle, currentMessage, ex);
				}
			}

			foreach (var module in _messageModules)
			{
				module.MessageProcessingFinished(messageBundle);
			}
		}
	}

	/// <summary>
	/// Convenient base class for implementors of <see cref="IMessageModule"/>
	/// </summary>
	public class MessageModule : IMessageModule
	{
		public virtual void Init()
		{
		}

		public virtual void Stop()
		{
		}

		public virtual void MessageArrived(MessageBundle messageBundle)
		{
		}

		public virtual void MessageProcessingStarting(MessageBundle messageBundle, ICollection<object> messageHandlers)
		{
		}

		public virtual void MessageProcessingSucceeded(MessageBundle messageBundle)
		{
		}

		public virtual void MessageProcessingFailure(MessageBundle messageBundle, object message, Exception ex)
		{
		}

		public virtual void MessageProcessingFinished(MessageBundle messageBundle)
		{
			
		}
	}

	[Singleton(typeof(IMessageModule))]
	public class CurrentMessageModule : MessageModule
	{
		private static readonly LocalDataStoreSlot Slot = Thread.AllocateDataSlot();

		public static MessageBundle CurrentMessageBundle { 
			get { return (MessageBundle) Thread.GetData(Slot); }
		}
			
		public override void MessageArrived(MessageBundle messageBundle)
		{
			Thread.SetData(Slot, messageBundle);
		}

		public override void MessageProcessingFinished(MessageBundle messageBundle)
		{
			Thread.SetData(Slot, null);
		}
	}


	public class MyPresenter : Presenter
	{
		public IBus Bus { get; set; }

		protected override void InitializePresenter()
		{
			BeginRefresh();
			SendMessage();
		}

		private void BeginRefresh()
		{
			IListQuery<ErrorReport> s = null;

			Bus.Query(s)
				.HandleResult(RefreshHandler)
				.HandleError(ErrorHandler);
		}

		private void RefreshHandler(IList<ErrorReport> list)
		{
			// Do something
		}

		private void ErrorHandler(ErrorReport errorReport)
		{
			// Do something
		}

		private void SendMessage()
		{
			var request = new {Name = "Fred", FavouriteColor = "Red"};
			Bus.Send(request)
				.HandleResponse<SomeResponse>(SomeResponseHandler)
				.HandleError(ErrorHandler);
		}

		private void SomeResponseHandler(SomeResponse someResponse)
		{
			
		}




	}

	public class SomeResponse
	{
		public int Result { get; set; }
	}
}
