﻿#region License

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Defines an interfaces to adapt an object and bind it to a new <see cref="IRegion"/>.
	/// </summary>
	public interface IRegionAdapter
	{
		/// <summary>
		/// Adapts an object and binds it to a new <see cref="IRegion"/>.
		/// </summary>
		/// <param name="regionTarget">The object to adapt.</param>
		/// <param name="regionName">The name of the region to be created.</param>
		/// <returns>The new instance of <see cref="IRegion"/> that the <paramref name="regionTarget"/> is bound to.</returns>
		IRegion Initialize(object regionTarget, string regionName);
	}
}
