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

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// A region that only allows one active view at a time.
	/// </summary>
	public abstract class SingleActiveRegion : Region
	{
		public override void Activate(object view)
		{
			foreach (object activeView in ActiveViews)
			{
				if (activeView != view && Views.Contains(activeView))
				{
					base.Deactivate(activeView);
				}
			}
			base.Activate(view);
		}
	}
}