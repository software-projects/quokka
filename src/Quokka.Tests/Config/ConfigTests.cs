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
				.With(with => with.DefaultValue("default-value")
				                  .Description("The description"));

			Assert.AreEqual("name", param.Name);
			Assert.AreEqual("default-value", param.Extra.DefaultValue);
			Assert.AreEqual("The description", param.Description);
		}

		[Test]
		public void Validation()
		{
			var param1 = new Int32Parameter("p1")
				.With(p => p.Validation(value => {
					if (value < 1 || value > 10)
					{
						return "valid values are in the range [1, 10]";
					}
					return null;
				}));

			var param2 = new Int32Parameter("p2");

			Assert.IsNull(param1.Extra.Validate(1));
			Assert.AreEqual("valid values are in the range [1, 10]", ((IConfigParameter<int>)param1).Validate(11));

			Assert.IsNull(param2.Extra.Validate(0));
			Assert.IsNull(param2.Extra.Validate(int.MaxValue));
		}

		[Test]
		public void ChangeValues()
		{
			IConfigParameter<int> param1 = new Int32Parameter("int32");
			Assert.AreEqual(0, param1.Value);
			param1.SetValue(2);
			Assert.AreEqual(2, param1.Value);

			IConfigParameter<string> param2 = new StringParameter("string");
			Assert.AreEqual(null, param2.Value);
			param2.SetValue("yyy");
			Assert.AreEqual("yyy", param2.Value);

			IConfigParameter<TimeSpan> param3 = new TimeSpanParameter("timespan");
			Assert.AreEqual(TimeSpan.Zero, param3.Value);
			param3.SetValue(TimeSpan.FromDays(1.5));
			Assert.AreEqual(TimeSpan.FromDays(1.5), param3.Value);
		}

		[Test]
		public void WhenChanged()
		{
			var changed = false;
			var param = new Int32Parameter("test1")
				.With(with => with.ChangeAction(() => changed = true));
			Assert.AreEqual(0, param.Value);

			((IConfigParameter<int>)param).SetValue(2);
			Assert.IsTrue(changed);
			Assert.AreEqual(2, param.Value);

			changed = false;
			((IConfigParameter<int>)param).SetValue(2);
			Assert.IsFalse(changed);
			Assert.AreEqual(2, param.Value);
		}

		[Test]
		public void ImplicitConversion()
		{
			var intp = new Int32Parameter("intp").With(p => p.DefaultValue(22));
			var intValue = intp*4;
			Assert.AreEqual(88, intValue);

			var stringp = new StringParameter("stringp").With(p => p.DefaultValue("Hello world"));
			var stringValue = stringp + "!";
			Assert.AreEqual("Hello world!", stringValue);
		}

		public void ScratchPad()
		{
			Int32Parameter param = new Int32Parameter("name")
				.With(p => p.Description("Some description")
				            .ValidRange(1, 10)
				            .DefaultValue(4)
							.ChangeAction(DoSomething));			
		}

		public static readonly Int32Parameter testParam = new Int32Parameter("test-param")
			.With(with => with.Description("Test parameter in Quokka.Core")
			                  .DefaultValue(1)
			                  .ValidRange(1, 10)
			                  .ChangeAction(() => Console.WriteLine("{0} changed to {1}", testParam.Name, testParam.Value)));

		private void DoSomething() {}
	}
}