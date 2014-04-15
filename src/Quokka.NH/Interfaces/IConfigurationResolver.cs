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

using NHibernate.Cfg;

namespace Quokka.NH.Interfaces
{
	public interface IConfigurationResolver : IDefaultAlias
	{
		/// <summary>
		/// Determines whether a configuration exists, or can be created, for the
		/// specified alias.
		/// </summary>
		/// <param name="alias">Alias of the configuration (which can be <c>null</c>).</param>
		/// <returns></returns>
		bool IsAliasDefined(string alias);

		/// <summary>
		/// Returns the <see cref="Configuration"/> associated with the specified alias (which can
		/// be <c>null</c>). If the configuration has not already been created, it will be created,
		/// which can be a lengthy operation.
		/// </summary>
		/// <param name="alias">Alias of the configuration (which can be <c>null</c>).</param>
		Configuration GetConfiguration(string alias);
	}
}