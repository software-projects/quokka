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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.WinForms.Commands;
using Quokka.WinForms.Interfaces;

namespace Quokka.WinForms.Controls
{
	public partial class MainMenuListView : UserControl, IMainMenu
	{
		private readonly List<IImageCommand> _commands = new List<IImageCommand>();
		private static readonly ILogger log = LoggerFactory.GetCurrentClassLogger();
		private readonly Dictionary<Image, int> _imageIndexes = new Dictionary<Image, int>();

		public MainMenuListView()
		{
			InitializeComponent();
			listView.Dock = DockStyle.Fill;
		}

		public void Add(IImageCommand command, string groupName)
		{
			if (InvokeRequired)
			{
				MethodInvoker action = () => Add(command, groupName);
				BeginInvoke(action);
				return;
			}

			if (command == null || _commands.Contains(command))
			{
				// Do nothing if the command has already been added.
				// Note that this prevents using the same command twice in different groups.
				return;
			}

			command.PropertyChanged += CommandPropertyChanged;

			ListViewItem item = new ListViewItem {Tag = command};
			Update(item);

			if (!string.IsNullOrEmpty(groupName))
			{
				ListViewGroup group = FindOrCreateGroup(groupName);
				item.Group = group;
			}
			listView.Items.Add(item);
		}

		public void Remove(IImageCommand command)
		{
			if (InvokeRequired)
			{
				MethodInvoker action = () => Remove(command);
				BeginInvoke(action);
				return;
			}

			ListViewItem item = FindByCommand(command);
			if (item != null)
			{
				command.PropertyChanged -= CommandPropertyChanged;
				listView.Items.Remove(item);
			}
		}

		private void Update(ListViewItem item)
		{
			IImageCommand command = item.Tag as IImageCommand;
			if (command == null)
			{
				log.Error("List view item does not have an associated command");
				return;
			}

			item.Text = command.Text;
			item.ImageIndex = GetImageIndex(command.ImageLarge);
		}

		private void CommandPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			IImageCommand command = sender as IImageCommand;
			if (command == null)
			{
				// this should not happen
				log.ErrorFormat("Received object of type {0} as sender to property changed event",
				                sender.GetType().FullName);
				return;
			}
			ListViewItem item = FindByCommand(command);
			if (item != null)
			{
				Update(item);
			}
		}

		private ListViewItem FindByCommand(IImageCommand command)
		{
			foreach (ListViewItem item in listView.Items)
			{
				IImageCommand itemCommand = item.Tag as IImageCommand;
				if (itemCommand == command)
				{
					return item;
				}
			}

			return null;
		}

		private ListViewGroup FindOrCreateGroup(string groupName)
		{
			foreach (ListViewGroup g in listView.Groups)
			{
				if (CaseInsensitiveComparer.Default.Compare(g.Header, groupName) == 0)
				{
					return g;
				}
			}
			ListViewGroup group = new ListViewGroup
			                      	{
			                      		Name = "listViewGroup" + listView.Groups.Count,
			                      		Header = groupName
			                      	};
			listView.Groups.Add(group);
			return group;
		}

		private void listView_ItemActivate(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0)
			{
				ListViewItem item = listView.SelectedItems[0];
				IImageCommand command = item.Tag as IImageCommand;
				if (command != null)
				{
					command.Execute();
				}
			}
		}

		private int GetImageIndex(Image image)
		{
			if (image == null)
			{
				return 0;
			}

			int index;
			if (!_imageIndexes.TryGetValue(image, out index))
			{
				index = largeImageList.Images.Count;
				largeImageList.Images.Add(image);
			}
			return index;
		}
	}
}