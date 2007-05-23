namespace Quokka.WinForms.Tests
{
	using System;
	using System.Windows.Forms;
	using Quokka.Uip;

	public partial class MessageBoxTestForm : Form
	{
		public MessageBoxTestForm() {
			InitializeComponent();
			messageTypeComboBox.SelectedIndex = 0;
		}

		private void displayButton_Click(object sender, EventArgs e) {
			UipQuestion question = new UipQuestion();
			question.MainInstruction = mainInstructionTextBox.Text;
			question.Content = contentTextBox.Text;
			question.QuestionType = (UipQuestionType)Enum.Parse(typeof(UipQuestionType), messageTypeComboBox.Text, true);
			//message.PossibleAnswers.Add(new UipAnswer(UipAnswerType.OK));
			//message.PossibleAnswers.Add(new UipAnswer(UipAnswerType.Cancel));
			question.PossibleAnswers.Add(new UipAnswer(UipAnswerType.Yes));
			question.PossibleAnswers.Add(new UipAnswer(UipAnswerType.No));
			question.PossibleAnswers.Add(new UipAnswer("Do another complicated thing"));
			MessageBoxForm messageBox = new MessageBoxForm();
			messageBox.Question = question;
			DialogResult dialogResult = messageBox.ShowDialog(this);
			UipAnswer pressedButton = messageBox.Question.SelectedAnswer;
			string text = String.Format("Return value = {0}, pressed button = {1}", dialogResult, pressedButton);
			MessageBox.Show(this, text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void button1_Click(object sender, EventArgs e) {
			string text = "Press yes or no";
			DialogResult result = MessageBox.Show(this, text, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
			MessageBox.Show(this, result.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}