using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Quokka.UI.Messages;

namespace Quokka.WinForms
{
	public partial class MessageBoxView : UserControl, IUIMessageBoxView
	{
		private UIMessage _message;

		public event EventHandler Answered;

		public MessageBoxView()
		{
			InitializeComponent();
			_message = CreateDefaultMessage();

			
		}

		public UIMessage Message
		{
			get { return _message; }
			set
			{
				_message = value;
			}
		}

		protected virtual void OnAnswered()
		{
			if (Answered != null)
			{
				Answered(this, EventArgs.Empty);
			}
		}

		private void ViewLoad(object sender, EventArgs e)
		{
			Text = Application.ProductName;
			// TODO: need to do something with the icon

			if (!DesignMode)
			{
				// During design it is easier if the borders are showing
				contentTextBox.BorderStyle = BorderStyle.None;
				mainInstructionTextBox.BorderStyle = BorderStyle.None;
				DisplayMessage();
				LoadButtons();
				CheckMainInstructionWidth();
				CheckContentSize();
				EnableWindowCloseButton();

				if (ParentForm != null)
				{

					// TODO: until we know what to do about showing icons?
					ParentForm.ShowIcon = false;

					ParentForm.FormClosing += ParentForm_FormClosing;
				}
			}
		}

		void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			var reason = e.CloseReason;
		}

		// Only allow a close button if there is a cancel option.
		private void EnableWindowCloseButton()
		{
			bool enable = _message.PossibleAnswers.Any(buttonSpec => buttonSpec.AnswerType == UIAnswerType.Cancel);
			var form = ParentForm;

			if (form != null)
			{
				Win32.SetWindowCloseButtonEnabled(form, enable);
			}
		}

		private static UIMessage CreateDefaultMessage()
		{
			var message = new UIMessage
			              	{
			              		MainInstruction = "Summary goes here",
			              		Content = "Content goes here",
			              		MessageType = UIMessageType.Information,
			              		PossibleAnswers =
			              			{
			              				new UIAnswer(UIAnswerType.OK)
			              			}
			              	};
			return message;
		}

		private void DisplayMessage()
		{
			mainInstructionTextBox.Text = _message.MainInstruction;
			contentTextBox.Text = _message.Content;
			pictureBox.Image = MessageBoxIcons.GetImage(_message.MessageType);
		}

		private void LoadButtons()
		{
			Button button = null;
			buttonFlowPanel.Controls.Clear();
			for (int index = _message.PossibleAnswers.Count - 1; index >= 0; --index)
			{
				button = CreateButton(_message.PossibleAnswers[index], index);
				buttonFlowPanel.Controls.Add(button);
			}

			if (button != null)
			{
				// adjust the width of the form to fit the buttons
				if (button.Left < mainInstructionTextBox.Left)
				{
					Width += mainInstructionTextBox.Left - button.Left;
				}
			}
		}

		private Button CreateButton(UIAnswer answer, int index)
		{
			Button button = new Button
			                	{
			                		AutoSize = true,
			                		Margin = new Padding(6),
			                		Name = "MessageButton" + index,
			                		Size = new Size(75, 23),
			                		TabIndex = index,
			                		Text = answer.ToString(),
			                		UseVisualStyleBackColor = true,
			                		Tag = answer,
			                	};
			button.Click += ButtonClick;
			return button;
		}

		private void CheckMainInstructionWidth()
		{
			using (Graphics graphics = CreateGraphics())
			{
				SizeF size = graphics.MeasureString(mainInstructionTextBox.Text, mainInstructionTextBox.Font);
				int requiredWidth = (int) Math.Ceiling(size.Width);
				if (mainInstructionTextBox.Width < requiredWidth)
				{
					Width += requiredWidth - mainInstructionTextBox.Width;
				}
			}
		}

		private void CheckContentSize()
		{
			Screen screen = Screen.FromControl(this);
			int maxHeight = (int) (screen.Bounds.Height*0.75);
			int maxWidth = (int) (screen.Bounds.Width*0.75);

			var parentForm = ParentForm;

			if (parentForm != null)
			{
				using (Graphics graphics = CreateGraphics())
				{
					for (;;)
					{
						SizeF layoutArea = new SizeF(contentTextBox.Width, maxHeight);
						SizeF size = graphics.MeasureString(contentTextBox.Text, contentTextBox.Font, layoutArea);
						int requiredHeight = (int) Math.Ceiling(size.Height);
						if (requiredHeight < contentTextBox.Height)
						{
							// Everything is fine. Take no action.
							return;
						}

						// Work out how big the form has to be.
						int requiredFormHeight = Height + requiredHeight - contentTextBox.Height;

						if (requiredFormHeight < Width || Width >= maxWidth)
						{
							parentForm.Height = requiredFormHeight;
							return;
						}

						// Increase the width of the form and recalculate
						parentForm.Width += maxWidth/5;
					}
				}
			}
		}

		private void ButtonClick(object sender, EventArgs e)
		{
			var button = (Button) sender;
			var answer = (UIAnswer) button.Tag;
			_message.SelectedAnswer = answer;
			OnAnswered();
		}
	}
}