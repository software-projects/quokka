﻿#region License

//  Notice: Some of the code in this file may have been adapted from code
//  in the Castle Project.
//
// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
using System.Runtime.Serialization;

namespace Quokka.NH.Startup
{
	/// <summary>
	/// Thrown when the <see cref="NHibernateFacility"/> detects a configuration problem.
	/// </summary>
	[Serializable]
	public class NHibernateFacilityException : Exception
	{
		public NHibernateFacilityException(string message) : base(message) { }
		public NHibernateFacilityException(string message, Exception ex) : base(message, ex) { }
		public NHibernateFacilityException() { }
		protected NHibernateFacilityException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
	}
}