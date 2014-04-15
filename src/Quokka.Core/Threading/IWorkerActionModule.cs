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

namespace Quokka.Threading
{
	public interface IWorkerActionModule
	{
		/// <summary>
		/// 	Called before the <see cref = "Worker" /> runs its <see cref = "WorkerAction" /> action.
		/// </summary>
		/// <remarks>
		///		The <see cref="Before"/> method for all <see cref="IWorkerActionModule"/> modules will
		///		be called prior to performing the <see cref="WorkerAction"/> action.
		/// </remarks>
		void Before();

		/// <summary>
		/// 	Called on successfuly completion of the <see cref = "WorkerAction" /> action. This method
		/// 	is not called if an exception is thrown.
		/// </summary>
		void After();

		/// <summary>
		/// 	Called if the <see cref = "WorkerAction" /> throws an exception while performing its work.
		/// </summary>
		/// <param name = "ex">The exception thrown</param>
		void Error(Exception ex);

		/// <summary>
		/// 	Called on completion of the <see cref = "WorkerAction" />, regardless of whether an
		/// 	exception was thrown or not.
		/// </summary>
		void Finished();
	}
}