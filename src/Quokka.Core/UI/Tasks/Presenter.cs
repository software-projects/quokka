#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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