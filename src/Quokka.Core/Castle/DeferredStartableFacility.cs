// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections.Generic;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;

namespace Quokka.Castle
{
	/// <summary>
	/// Custom version of the Castle startable facility. Instead of starting
	/// components as soon as possible, this facility stores all startable
	/// components in the wait list, and starts them when the Start method is called.
	/// Also generates warnings for startable components that cannot be instantiated.
	/// </summary>
	/// <remarks>
	/// It's a pity, but the StartableFacility does not lend itself to subtyping,
	/// as everything we need is private, so we duplicate a bit of code here.
	/// </remarks>
	public class DeferredStartableFacility : AbstractFacility
	{
		private readonly List<IHandler> waitList = new List<IHandler>();
		private ITypeConverter converter;

		// Don't check the waiting list while this flag is set as this could result in
		// duplicate singletons.
		private bool disableException;
		private bool inStart;
		private bool optimizeForSingleInstall;

		protected override void Init()
		{
			optimizeForSingleInstall = true;
			disableException = false;
			converter = Kernel.GetConversionManager();
			Kernel.ComponentModelBuilder.AddContributor(new StartableContributor(converter));
			if (optimizeForSingleInstall)
			{
				Kernel.ComponentRegistered += CacheForStart;
				return;
			}
			Kernel.ComponentRegistered += OnComponentRegistered;
		}

		private void AddHandlerToWaitingList(IHandler handler)
		{
			waitList.Add(handler);
		}

		private void CacheForStart(string key, IHandler handler)
		{
			if (IsStartable(handler))
			{
				waitList.Add(handler);
			}
		}

		/// <summary>
		///   For each new component registered,
		///   some components in the WaitingDependency
		///   state may have became valid, so we check them
		/// </summary>
		private void CheckWaitingList()
		{
			if (!inStart)
			{
				var handlers = waitList.ToArray();
				foreach (var handler in handlers)
				{
					if (TryStart(handler))
					{
						waitList.Remove(handler);
					}
				}
			}
		}

		private bool IsStartable(IHandler handler)
		{
			var startable = handler.ComponentModel.ExtendedProperties["startable"];
			var isStartable = (bool?)startable;
			return isStartable.GetValueOrDefault();
		}

		private void OnComponentRegistered(String key, IHandler handler)
		{
			var startable = IsStartable(handler);
			if (startable)
			{
				if (TryStart(handler) == false)
				{
					AddHandlerToWaitingList(handler);
				}
			}

			CheckWaitingList();
		}

		private void Start(IHandler handler)
		{
			handler.Resolve(CreationContext.CreateEmpty());
		}

		public void StartAll()
		{
			var array = waitList.ToArray();
			waitList.Clear();
			foreach (var handler in array)
			{
				if (disableException == false)
				{
					Start(handler);
					continue;
				}

				if (TryStart(handler) == false)
				{
					AddHandlerToWaitingList(handler);
				}
			}
		}

		/// <summary>
		///   Request the component instance
		/// </summary>
		/// <param name = "handler"></param>
		private bool TryStart(IHandler handler)
		{
			try
			{
				inStart = true;
				return handler.TryResolve(CreationContext.CreateEmpty()) != null;
			}
			finally
			{
				inStart = false;
			}
		}
	}
}