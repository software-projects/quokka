using System;
using NUnit.Framework;
using Quokka.Config.Storage;

// ReSharper disable InconsistentNaming
namespace Quokka.Config
{
	[TestFixture]
	public class ConfigTests
	{
		[TearDown]
		public void TearDown()
		{
			// clear out any parameters from previous tests
			ConfigParameter.Storage = new MemoryStorage();
		}

		[Test]
		public void Name_defaultValue_and_description()
		{
			var param = new StringParameter("name")
				.WithDefaultValue("default-value")
				.WithDescription("The description");

			Assert.AreEqual("name", param.Name);
			Assert.AreEqual("default-value", param.GetDefaultValue());
			Assert.AreEqual("The description", param.Description);
		}

		[Test]
		public void Validation()
		{
			var param1 = new Int32Parameter("p1")
				.WithValidation(value => {
					if (value < 1 || value > 10)
					{
						return "valid values are in the range [1, 10]";
					}
					return null;
				});

			var param2 = new Int32Parameter("p2");

			Assert.IsNull(param1.Validate(1));
			Assert.AreEqual("valid values are in the range [1, 10]", param1.Validate(11));

			Assert.IsNull(param2.Validate(0));
			Assert.IsNull(param2.Validate(int.MaxValue));
		}

		[Test]
		public void ChangeValues()
		{
			var param1 = new Int32Parameter("int32");
			Assert.AreEqual(0, param1.Value);
			param1.SetValue(2);
			Assert.AreEqual(2, param1.Value);

			var param2 = new StringParameter("string");
			Assert.AreEqual(null, param2.Value);
			param2.SetValue("yyy");
			Assert.AreEqual("yyy", param2.Value);

			var param3 = new TimeSpanParameter("timespan");
			Assert.AreEqual(TimeSpan.Zero, param3.Value);
			param3.SetValue(TimeSpan.FromDays(1.5));
			Assert.AreEqual(TimeSpan.FromDays(1.5), param3.Value);
		}

		[Test]
		public void WhenChanged()
		{
			var changed = false;
			var param = new Int32Parameter("test1")
				.WhenChanged(() => changed = true);
			Assert.AreEqual(0, param.Value);
			param.SetValue(2);
			Assert.IsTrue(changed);
			Assert.AreEqual(2, param.Value);

			changed = false;
			param.SetValue(2);
			Assert.IsFalse(changed);
			Assert.AreEqual(2, param.Value);
		}
	}
}