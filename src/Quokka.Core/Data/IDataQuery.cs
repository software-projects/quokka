using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Quokka.Data
{
	public interface IListQuery<T>
	{
		IList<T> ExecuteList();
	}

	public interface IBus
	{
		IBusQueryHandler<T> Query<T>(IListQuery<T> query);
		IBusResponseHandler Request<T>(T request);
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
			Bus.Request(request)
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
