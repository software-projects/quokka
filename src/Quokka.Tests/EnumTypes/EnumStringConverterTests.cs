using NUnit.Framework;
using Quokka.Attributes;

namespace Quokka.EnumTypes
{
	[TestFixture]
	public class EnumStringConverterTests
	{
		[Test]
		public void CanHandleNormalEnum()
		{
			Assert.AreEqual(6, EnumStringConverter<TestEnum1>.MaxLength);
			Assert.AreEqual("X", EnumStringConverter<TestEnum1>.ConvertToString(TestEnum1.Value1));
			Assert.AreEqual("Value2", EnumStringConverter<TestEnum1>.ConvertToString(TestEnum1.Value2));
			Assert.AreEqual("Y", EnumStringConverter<TestEnum1>.ConvertToString(TestEnum1.Value3));
			Assert.AreEqual(TestEnum1.Value1, EnumStringConverter<TestEnum1>.ConvertToEnum("x"));
			Assert.AreEqual(TestEnum1.Value2, EnumStringConverter<TestEnum1>.ConvertToEnum("value2"));
			Assert.AreEqual(TestEnum1.Value2, EnumStringConverter<TestEnum1>.ConvertToEnum("VALUE2"));
			Assert.AreEqual(TestEnum1.Value3, EnumStringConverter<TestEnum1>.ConvertToEnum("Y"));
		}

		[Test]
		public void CanHandleNullStringValue()
		{
			Assert.AreEqual(2, EnumStringConverter<TestEnum2>.MaxLength);
			Assert.IsNull(EnumStringConverter<TestEnum2>.ConvertToString(TestEnum2.V1));
			Assert.AreEqual(TestEnum2.V1, EnumStringConverter<TestEnum2>.ConvertToEnum(null));
			Assert.AreEqual("M", EnumStringConverter<TestEnum2>.ConvertToString(TestEnum2.AnotherValue));
			Assert.AreEqual("CC", EnumStringConverter<TestEnum2>.ConvertToString(TestEnum2.Value3));
		}
	}

	public enum TestEnum1
	{
		[StringValue("X")]
		Value1,

		Value2,

		[StringValue("Y")]
		Value3
	}

	public enum TestEnum2
	{
		[StringValue(null)]
		V1,

		[StringValue("M")]
		AnotherValue,

		[StringValue("CC")]
		Value3,
	}
}
