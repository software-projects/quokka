using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace Quokka.Server.Internal
{
	internal class StopManager : IStopManager, IDisposable
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof (StopManager));
		private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
		private bool _hasStopBeenRequested;
		private readonly object _lockObject = new object();
		private readonly RegisteredWaitHandle _registeredWaitHandle;
		private readonly List<Action> _stopActions = new List<Action>();
		private int _delayStopCount;

		public StopManager()
		{
			_registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_stopEvent, StopRequestedCallback, null, -1, true);
		}

		public void Dispose()
		{
			_registeredWaitHandle.Unregister(_stopEvent);
			_stopEvent.Close();
		}

		public void RequestStop()
		{
			Logger.Info("Program stopping");
			bool setStopEvent = false;

			lock (_lockObject)
			{
				if (!_hasStopBeenRequested)
				{
					_hasStopBeenRequested = true;
					setStopEvent = true;
				}
			}

			if (setStopEvent)
			{
				_stopEvent.Set();
			}
		}

		public bool HasStopBeenRequested
		{
			get { return _hasStopBeenRequested; }
		}

		public WaitHandle StopWaitHandle
		{
			get { return _stopEvent; }
		}

		public WaitHandle StoppedWaitHandle
		{
			get { return _stoppedEvent; }
		}

		public void RegisterStopAction(Action stopAction)
		{
			if (stopAction != null)
			{
				_stopActions.Add(stopAction);
			}
		}

		public IDisposable DelayStop()
		{
			IncrementDelayStop();
			return new DisposableAction(DecrementDelayStop);
		}

		private void IncrementDelayStop()
		{
			lock (_lockObject)
			{
				++_delayStopCount;
			}
		}

		private void DecrementDelayStop()
		{
			bool setStoppedEvent = false;

			lock (_lockObject)
			{
				--_delayStopCount;
				if (_delayStopCount <= 0)
				{
					setStoppedEvent = true;
				}
			}

			if (setStoppedEvent)
			{
				_stoppedEvent.Set();
			}
		}

		private void StopRequestedCallback(object state, bool timedOut)
		{
			foreach (var action in _stopActions)
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					// TODO Log error message here
				}
			}

			bool setStoppedEvent = false;
			lock (_lockObject)
			{
				if (_delayStopCount <= 0)
				{
					setStoppedEvent = true;
				}
			}

			if (setStoppedEvent)
			{
				_stoppedEvent.Set();
			}
		}
	}
}
