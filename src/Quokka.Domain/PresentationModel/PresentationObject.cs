#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2008 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System;
using System.ComponentModel;
using System.Threading;

namespace Quokka.PresentationModel
{
	public class PresentationObject : INotifyPropertyChanged
	{
		private int _updateCount;
		private bool _isSelected;

		// The name of a property that has changed during an update.
		// If more than one property has changed, this is the empty string "".
		// If no properties have changed, this is null.
		private string _changedPropertyName;

		public event PropertyChangedEventHandler PropertyChanged;

		public int Id { get; protected set; }

		
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					NotifyPropertyChanged("IsSelected");
				}
			}
		}

		public bool IsInUpdate
		{
			get { return _updateCount > 0; }
		}

		public IDisposable BeginUpdate()
		{
			Interlocked.Add(ref _updateCount, 1);
			return new DisposableUpdate(this);
		}

		public void EndUpdate()
		{
			int result = Interlocked.Decrement(ref _updateCount);
			if (result < 0)
			{
				throw new PresentationModelException("Mismatch BeginUpdate and EndUpdate");
			}
			if (result == 0)
			{
				if (_changedPropertyName != null)
				{
					RaisePropertyChangedIfChanged(_changedPropertyName);
				}
				_changedPropertyName = null;
			}
		}

		protected static bool HasValueChanged<T>(T oldValue, T newValue) where T : IEquatable<T>
		{
			IEquatable<T> comparer1 = oldValue;
			bool hasValueChanged;

			if (comparer1 == null)
			{
				IEquatable<T> comparer2 = newValue;
				hasValueChanged = (comparer2 != null);
			}
			else
			{
				hasValueChanged = !comparer1.Equals(newValue);
			}

			return hasValueChanged;
		}

		// Called from the NotifyChangedAttribute class.
		internal static void RaisePropertyChangedEventIfNecessary(object obj, object oldValue, object newValue, string propertyName)
		{
			PresentationObject po = obj as PresentationObject;
			if (po == null)
			{
				// TODO: log an error message here
				return;
			}


			// do nothing if old value and new value are both null
			if (oldValue == null && newValue == null)
				return;

			if (oldValue == null || newValue == null)
			{
				// one of the values is null and the other isn't, so raise an event
				po.NotifyPropertyChanged(propertyName);
				return;
			}

			// at this point both values are not null
			if (!oldValue.Equals(newValue))
			{
				po.NotifyPropertyChanged(propertyName);
			}

			return;
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (IsInUpdate)
			{
				if (_changedPropertyName == null)
				{
					// no other properties changed yet in this update
					_changedPropertyName = propertyName;
				}
				else if (_changedPropertyName != propertyName)
				{
					// a different property name has been changed in this update,
					// so the changed property name now becomes an empty string to mean "all properties"
					_changedPropertyName = String.Empty;
				}
			}
			else
			{
				// not in an update, so raise the event directly
				RaisePropertyChangedIfChanged(propertyName);
			}
		}

		private void RaisePropertyChangedIfChanged(string propertyName)
		{
			// normalise empty string to null
			if (propertyName != null && propertyName.Length == 0)
			{
				propertyName = null;
			}

			// TODO: as an optimisation, could keep a dictionary of property event args, as they are immutable,
			// would save lots of object allocations
			PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
			OnPropertyChanged(e);
		}

		// Allows derived classes to override
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		#region class DisposableUpdate

		private class DisposableUpdate : IDisposable
		{
			private readonly PresentationObject _obj;

			public DisposableUpdate(PresentationObject obj)
			{
				_obj = obj;
			}

			public void Dispose()
			{
				_obj.EndUpdate();
			}
		}

		#endregion
	}
}