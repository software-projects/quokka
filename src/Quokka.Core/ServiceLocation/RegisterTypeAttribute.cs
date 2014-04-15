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

using System;
using Quokka.Diagnostics;

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Attribute for conveniently registering services with the service container.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RegisterTypeAttribute : Attribute
	{
		private readonly Type _type;
		private readonly ServiceLifecycle _lifecycle;

		public RegisterTypeAttribute(Type type, ServiceLifecycle lifecycle)
		{
			Verify.ArgumentNotNull(type, "type", out _type);
			_lifecycle = lifecycle;
		}

		public Type Type
		{
			get { return _type; }
		}

		public ServiceLifecycle Lifecycle
		{
			get { return _lifecycle; }
		}

		public string Name { get; set; }
	}
}