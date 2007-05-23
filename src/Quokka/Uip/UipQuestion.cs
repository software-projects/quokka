//
// THIS WHOLE FILE IS AN EXPERIMENT
// DON'T USE IT YET.
//
namespace Quokka.Uip
{
	using System;
	using System.Collections.Generic;

	public enum UipQuestionType
	{
		Information,
		Success,
		Question,
		Warning,
		Forbidden,
		Unauthorized,
		Failure,
	}

	public enum UipAnswerType
	{
		// These numbers match the values in System.Windows.Forms.DialogResult
		Custom = 0,
		OK = 1,
		Cancel = 2,
		Abort = 3,
		Retry = 4,
		Ignore = 5,
		Yes = 6,
		No = 7,
	}

	public class UipQuestion
	{
		private UipQuestionType _questionType;
		private string _mainInstruction;
		private string _content;
		private readonly List<UipAnswer> _possibleAnswers;
		private UipAnswer _selectedAnswer;

		public UipQuestion()
		{
			_possibleAnswers = new List<UipAnswer>();
		}

		public UipQuestionType QuestionType
		{
			get { return _questionType; }
			set { _questionType = value; }
		}

		public string MainInstruction
		{
			get { return _mainInstruction; }
			set { _mainInstruction = value; }
		}

		public string Content
		{
			get { return _content; }
			set { _content = value; }
		}

		public UipAnswer SelectedAnswer
		{
			get { return _selectedAnswer; }
			set { _selectedAnswer = value; }
		}

		public IList<UipAnswer> PossibleAnswers
		{
			get { return _possibleAnswers; }
		}
	}

	public class UipAnswer
	{
		private readonly string _text;
		private readonly UipAnswerType _answerType;

		public UipAnswer(string text) {
			_text = text;
			_answerType = UipAnswerType.Custom;
		}

		public UipAnswer(UipAnswerType answerType) {
			if (answerType == UipAnswerType.Custom) {
				throw new ArgumentException("Must supply text for a custom button");
			}
			_text = answerType.ToString();
			_answerType = answerType;
		}

		public UipAnswerType AnswerType
		{
			get { return _answerType; }
		}

		public override string ToString() {
			return _text;
		}
	}
}
