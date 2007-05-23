namespace Quokka.WinForms
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using Quokka.Uip.Interactive;

	public partial class MessageBoxForm : Form
	{
		private UipMessage uipMessage;

		public MessageBoxForm()
		{
			InitializeComponent();
			uipMessage = CreateDefaultMessage();
		}

		public UipMessage Message
		{
			get { return uipMessage; }
			set
			{
				if (uipMessage == null) {
					throw new ArgumentNullException();
				}
				uipMessage = value;
			}
		}

		private void MessageBoxForm_Load(object sender, EventArgs e)
		{
			Text = Application.ProductName;
			// TODO: need to do something with the icon

			if (!DesignMode) {
				// During design it is easier if the borders are showing
				contentTextBox.BorderStyle = BorderStyle.None;
				mainInstructionTextBox.BorderStyle = BorderStyle.None;
				DisplayMessage();
				LoadButtons();
				CheckMainInstructionWidth();
				CheckContentSize();
				EnableWindowCloseButton();
				CenterToParent();
			}
		}

		private void EnableWindowCloseButton()
		{
			bool enable = false;

			foreach (UipMessageButton buttonSpec in uipMessage.MessageButtons)
			{
				if (buttonSpec.ButtonType == ButtonType.Cancel) {
					enable = true;
					break;
				}
			}

			Win32.SetWindowCloseButtonEnabled(this, enable);
		}

		private UipMessage CreateDefaultMessage()
		{
			UipMessage message = new UipMessage();
			message.MainInstruction = "Main instruction goes here";
			message.Content = "Content goes here";
			message.MessageType = MessageType.Information;
			message.MessageButtons.Add(new UipMessageButton(ButtonType.OK));
			return message;
		}

		private void DisplayMessage()
		{
			mainInstructionTextBox.Text = uipMessage.MainInstruction;
			contentTextBox.Text = uipMessage.Content;
			switch (uipMessage.MessageType) {
				case MessageType.Information:
					pictureBox.Image = Properties.Resources.Information;
					break;
				case MessageType.Success:
					pictureBox.Image = Properties.Resources.Success;
					break;
				case MessageType.Question:
					pictureBox.Image = Properties.Resources.Question;
					break;
				case MessageType.Warning:
					pictureBox.Image = Properties.Resources.Warning;
					break;
				case MessageType.Forbidden:
					pictureBox.Image = Properties.Resources.Forbidden;
					break;
				case MessageType.Unauthorized:
					pictureBox.Image = Properties.Resources.NoEntry;
					break;
				case MessageType.Failure:
				default:
					pictureBox.Image = Properties.Resources.Failure;
					break;
			}
		}

		private void LoadButtons()
		{
			Button button = null;
			buttonFlowPanel.Controls.Clear();
			for (int index = uipMessage.MessageButtons.Count - 1; index >= 0; --index) {
				button = CreateButton(uipMessage.MessageButtons[index], index);
				buttonFlowPanel.Controls.Add(button);
			}

			if (button != null) {
				// adjust the width of the form to fit the buttons
				if (button.Left < mainInstructionTextBox.Left) {
					Width += mainInstructionTextBox.Left - button.Left;
				}
			}
		}

		private Button CreateButton(UipMessageButton specification, int index)
		{
			Button button = new Button();
			button.AutoSize = true;
			button.Margin = new System.Windows.Forms.Padding(6);
			button.Name = "MessageButton" + index;
			button.Size = new System.Drawing.Size(75, 23);
			button.TabIndex = index;
			button.Text = specification.ToString();
			button.UseVisualStyleBackColor = true;
			button.Tag = specification;
			button.Click += new EventHandler(Button_Click);
			// TODO: this assumes that DialogResult and ButtonType values are the same (which they are at the moment).
			button.DialogResult = (DialogResult)specification.ButtonType;
			return button;
		}

		private void CheckMainInstructionWidth()
		{
			using (Graphics graphics = CreateGraphics()) {
				SizeF size = graphics.MeasureString(mainInstructionTextBox.Text, mainInstructionTextBox.Font);
				int requiredWidth = (int)Math.Ceiling(size.Width);
				if (mainInstructionTextBox.Width < requiredWidth) {
					Width += requiredWidth - mainInstructionTextBox.Width;
				}
			}
		}

		private void CheckContentSize()
		{
			Screen screen = Screen.FromControl(this);
			int maxHeight = (int)(screen.Bounds.Height * 0.75);
			int maxWidth = (int)(screen.Bounds.Width * 0.75);

			using (Graphics graphics = CreateGraphics()) {
				for (;;) {
					SizeF layoutArea = new SizeF(contentTextBox.Width, maxHeight);
					SizeF size = graphics.MeasureString(contentTextBox.Text, contentTextBox.Font, layoutArea);
					int requiredHeight = (int)Math.Ceiling(size.Height);
					if (requiredHeight < contentTextBox.Height) {
						// Everything is fine. Take no action.
						return;
					}

					// Work out how big the form has to be.
					int requiredFormHeight = Height + requiredHeight - contentTextBox.Height;

					if (requiredFormHeight < Width || Width >= maxWidth) {
						Height = requiredFormHeight;
						return;
					}

					// Increase the width of the form and recalculate
					Width += maxWidth / 5;
				}
			}
		}

		private void Button_Click(object sender, EventArgs e)
		{
			Button button = (Button)sender;
			UipMessageButton buttonSpec = (UipMessageButton)button.Tag;
			uipMessage.PressedButton = buttonSpec;
			DialogResult = (DialogResult)buttonSpec.ButtonType;
			Close();
		}
	}
}