namespace Quokka.WinForms.Testing
{
	using System;

	/// <summary>
	/// Attribute class used in test programs to link test controllers
	/// with views.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Use this attribute in your test programs to quickly identify
	/// which view (or views) a test controller is designed to test.
	/// </para>
	/// <para>
	/// This class is intended only for test programs.
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public class TestsViewAttribute : Attribute
	{
		private readonly Type _viewType;
		private string _comment;

		public TestsViewAttribute(Type viewType)
		{
			_viewType = viewType;
		}

		/// <summary>
		/// The type of view that this controller is designed to test.
		/// </summary>
		public Type ViewType
		{
			get { return _viewType; }
		}

		/// <summary>
		/// An optional comment to describe the kind of testing
		/// the controller provides.
		/// </summary>
		/// <remarks>
		/// Useful when the same view is tested by more than one
		/// test controller.
		/// </remarks>
		public string Comment
		{
			get { return _comment; }
			set { _comment = value; }
		}
	}
}
