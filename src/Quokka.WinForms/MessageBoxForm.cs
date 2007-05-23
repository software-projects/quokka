namespace Quokka.WinForms
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using Quokka.Uip;

	public partial class MessageBoxForm : Form
	{
		private UipQuestion _uipQuestion;

		public MessageBoxForm()
		{
			InitializeComponent();
			_uipQuestion = CreateDefaultMessage();
		}

		public UipQuestion Question
		{
			get { return _uipQuestion; }
			set
			{
				if (_uipQuestion == null) {
					throw new ArgumentNullException();
				}
				_uipQuestion = value;
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

			foreach (UipAnswer buttonSpec in _uipQuestion.PossibleAnswers)
			{
				if (buttonSpec.AnswerType == UipAnswerType.Cancel) {
					enable = true;
					break;
				}
			}

			Win32.SetWindowCloseButtonEnabled(this, enable);
		}

		private UipQuestion CreateDefaultMessage()
		{
			UipQuestion question = new UipQuestion();
			question.MainInstruction = "Main instruction goes here";
			question.Content = "Content goes here";
			question.QuestionType = UipQuestionType.Information;
			question.PossibleAnswers.Add(new UipAnswer(UipAnswerType.OK));
			return question;
		}

		private void DisplayMessage()
		{
			mainInstructionTextBox.Text = _uipQuestion.MainInstruction;
			contentTextBox.Text = _uipQuestion.Content;
			switch (_uipQuestion.QuestionType) {
				case UipQuestionType.Information:
					pictureBox.Image = Properties.Resources.Information;
					break;
				case UipQuestionType.Success:
					pictureBox.Image = Properties.Resources.Success;
					break;
				case UipQuestionType.Question:
					pictureBox.Image = Properties.Resources.Question;
					break;
				case UipQuestionType.Warning:
					pictureBox.Image = Properties.Resources.Warning;
					break;
				case UipQuestionType.Forbidden:
					pictureBox.Image = Properties.Resources.Forbidden;
					break;
				case UipQuestionType.Unauthorized:
					pictureBox.Image = Properties.Resources.NoEntry;
					break;
				case UipQuestionType.Failure:
				default:
					pictureBox.Image = Properties.Resources.Failure;
					break;
			}
		}

		private void LoadButtons()
		{
			Button button = null;
			buttonFlowPanel.Controls.Clear();
			for (int index = _uipQuestion.PossibleAnswers.Count - 1; index >= 0; --index) {
				button = CreateButton(_uipQuestion.PossibleAnswers[index], index);
				buttonFlowPanel.Controls.Add(button);
			}

			if (button != null) {
				// adjust the width of the form to fit the buttons
				if (button.Left < mainInstructionTextBox.Left) {
					Width += mainInstructionTextBox.Left - button.Left;
				}
			}
		}

		private Button CreateButton(UipAnswer answer, int index)
		{
			Button button = new Button();
			button.AutoSize = true;
			button.Margin = new System.Windows.Forms.Padding(6);
			button.Name = "MessageButton" + index;
			button.Size = new System.Drawing.Size(75, 23);
			button.TabIndex = index;
			button.Text = answer.ToString();
			button.UseVisualStyleBackColor = true;
			button.Tag = answer;
			button.Click += new EventHandler(Button_Click);
			// TODO: this assumes that DialogResult and Answer values are the same (which they are at the moment).
			button.DialogResult = (DialogResult)answer.AnswerType;
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
			UipAnswer answer = (UipAnswer)button.Tag;
			_uipQuestion.SelectedAnswer = answer;
			DialogResult = (DialogResult)answer.AnswerType;
			Close();
		}
	}
}