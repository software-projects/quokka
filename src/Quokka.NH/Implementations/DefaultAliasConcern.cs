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
using Castle.Core;
using Quokka.NH.Interfaces;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Implements <see cref="ICommissionConcern"/> in order to populate the
	/// <see cref="IDefaultAlias.DefaultAlias"/> property on components that
	/// implement the <see cref="IDefaultAlias"/> interface.
	/// </summary>
	public class DefaultAliasConcern : ICommissionConcern
	{
		private readonly Func<string> _getDefaultAlias;

		public DefaultAliasConcern(Func<string> getDefaultAlias)
		{
			_getDefaultAlias = getDefaultAlias;
		}

		public void Apply(ComponentModel model, object component)
		{
			var defaultAlias = component as IDefaultAlias;
			if (defaultAlias != null && _getDefaultAlias != null)
			{
				defaultAlias.DefaultAlias = _getDefaultAlias();
			}
		}
	}
}