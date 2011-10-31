using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.UI.Messages
{
	public class UIAnswer
	{
		private readonly string _text;
		private readonly UIAnswerType _answerType;

		public UIAnswer(string text)
		{
			_text = text;
			_answerType = UIAnswerType.Custom;
		}

		public UIAnswer(UIAnswerType answerType)
		{
			if (answerType == UIAnswerType.Custom)
			{
				throw new ArgumentException("Must supply text for a custom button");
			}
			_text = answerType.ToString();
			_answerType = answerType;
		}

		public UIAnswerType AnswerType
		{
			get { return _answerType; }
		}

		public Action Callback { get; set; }

		public override string ToString()
		{
			return _text;
		}
	}
}
