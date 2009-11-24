using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NUnit.Framework;

namespace Quokka.PresentationModel
{
	[TestFixture]
    [Ignore("PostSharp not installed on John W's machine for now")]
	public class NotifyChangedTests
	{
		private bool _eventRaised;
		private string _propertyName;

		[SetUp]
		public void SetUp()
		{
			ClearEvent();
		}

		[Test]
		public void RaisesEventWhenSinglePropertyValueChanged()
		{
			PresObj po = new PresObj();
			po.PropertyChanged += OnPropertyChanged;

			ClearEvent();
			po.Name = null;
			AssertEventNotRaised();

			po.Name = "Some value";
			AssertEventRaised("Name");

			ClearEvent();
			po.Name = "Some value";
			AssertEventNotRaised();

			ClearEvent();
			po.Name = "Some other value";
			AssertEventRaised("Name");

			ClearEvent();
			po.Number = 99;
			AssertEventRaised("Number");

			ClearEvent();
			po.Date = null;
			AssertEventNotRaised();

			ClearEvent();
			po.Date = DateTime.Now;
			AssertEventRaised("Date");

			ClearEvent();
			po.Date = DateTime.Today - new TimeSpan(1, 0, 0, 0);
			AssertEventRaised("Date");

			ClearEvent();
			po.Date = null;
			AssertEventRaised("Date");
		}

		[Test]
		public void RaisesEventWhenSinglePropertyValueChangedInUpdate()
		{
			PresObj po = new PresObj();
			po.PropertyChanged += OnPropertyChanged;

			using (po.BeginUpdate())
			{
				po.Name = "New value";
				AssertEventNotRaised();
			}

			AssertEventRaised("Name");
		}

		[Test]
		public void RaisesEventWhenMultiplePropertyValuesChangedInUpdate()
		{
			PresObj po = new PresObj();
			po.PropertyChanged += OnPropertyChanged;

			using (po.BeginUpdate())
			{
				po.Name = "New value";
				po.Number = 42;
				AssertEventNotRaised();
			}

			AssertEventRaised(null);
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_eventRaised = true;
			_propertyName = e.PropertyName;
		}

		private void ClearEvent()
		{
			_eventRaised = false;
			_propertyName = null;
		}

		private void AssertEventNotRaised()
		{
			Assert.IsFalse(_eventRaised);
		}

		private void AssertEventRaised(string propertyName)
		{
			Assert.IsTrue(_eventRaised, "Expected PropertyChanged event to have been raised");
			Assert.AreEqual(propertyName, _propertyName);
		}

		
		public class PresObj : PresentationObject
		{
			[NotifyChanged]
			public string Name { get; set; }

			[NotifyChanged]
			public int Number { get; set; }

			[NotifyChanged]
			public DateTime? Date { get; set; }
		}
	}
}
