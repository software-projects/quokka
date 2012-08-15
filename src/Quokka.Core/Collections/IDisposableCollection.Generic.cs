#region License

// Copyright 2004-2012 John Jeffery <john@jeffery.id.au>
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

using System;
using System.Collections.Generic;

namespace Quokka.Collections
{
	/// <summary>
	/// 	A collection of objects that are disposable. The collection itself implements <see cref = "IDisposable" />
	/// </summary>
	/// <typeparam name = "T">Type of objects in the collection</typeparam>
	public interface IDisposableCollection<T> : IDisposable, ICollection<T> where T : IDisposable
	{
	}
}