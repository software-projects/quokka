namespace Quokka.WinForms.Tests
{
	using System;
	using System.Windows.Forms;
	using Quokka.Uip.Interactive;

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
			question.MessageType = (MessageType)Enum.Parse(typeof(MessageType), messageTypeComboBox.Text, true);
			//message.PossibleAnswers.Add(new UipMessageButton(ButtonType.OK));
			//message.PossibleAnswers.Add(new UipMessageButton(ButtonType.Cancel));
			question.MessageButtons.Add(new UipMessageButton(ButtonType.Yes));
			question.MessageButtons.Add(new UipMessageButton(ButtonType.No));
			question.MessageButtons.Add(new UipMessageButton("Do another complicated thing"));
			MessageBoxForm messageBox = new MessageBoxForm();
			messageBox.Question = question;
			DialogResult dialogResult = messageBox.ShowDialog(this);
			UipMessageButton pressedButton = messageBox.Question.PressedButton;
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