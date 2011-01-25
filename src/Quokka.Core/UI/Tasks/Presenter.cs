namespace Quokka.UI.Tasks
{
	///<summary>
	///	Presenter class, where the view type has not been specified at compile time.
	///	This class is seldom used. Most presenters should derive from the 
	///	<see cref = "Presenter{T}" /> class.
	///</summary>
	///<remarks>
	///	<para>
	///		Use this as a base class for presenters that do not have an associated view, or
	///		for presenters whose view type is not known at compile time, and is dynamically
	///		determined at runtime. Both of these use cases are fairly rare.
	///	</para>
	///	<para>
	///		Most presenters should derive from the <see cref = "Presenter{T}" /> class.
	///	</para>
	///</remarks>
	public abstract class Presenter : PresenterBase
	{
		public object View
		{
			get { return ViewObject; }
			set { ViewObject = value; }
		}
	}
}