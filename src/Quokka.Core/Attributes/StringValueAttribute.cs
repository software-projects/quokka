using System;

namespace Quokka.Attributes
{
	/// <summary>
	/// Specify a string value to associate with an enumerated value.
	/// </summary>
	/// <remarks>
	/// Use this attribute when you wish to associate a string value with an
	/// enumerated value
	/// </remarks>
	/// <example>
	/// <code>
	/// enum Sex {
	///   [StringValue("F")] Female,
	///   [StringValue("M")] Male
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// The following example shows an enumerated type where one of the 
	/// values corresponds to a null value. Typically, if an enum maps
	/// to a null string, then this should be the default value for the enum type.
	/// <code>
	/// enum Answer {
	///   [StringValue(null)] NotAnswered,
	///   [StringValue("Y")] Yes,
	///   [StringValue("N")] No,
	///   [StringValue("U")] Unsure,
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field)]
	public class StringValueAttribute : Attribute
	{
		private readonly string _stringValue;

		public StringValueAttribute(string stringValue)
		{
			_stringValue = stringValue;
		}

		public string StringValue
		{
			get { return _stringValue; }
		}
	}
}
