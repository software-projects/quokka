using System.Collections.Generic;

namespace Quokka.UI.Messages
{
	public class UIAnswerCollection : List<UIAnswer>
	{
		public UIAnswer Add(UIAnswerType answerType)
		{
			UIAnswer answer = new UIAnswer(answerType);
			Add(answer);
			return answer;
		}

		public UIAnswer Add(string text)
		{
			UIAnswer answer = new UIAnswer(text);
			Add(answer);
			return answer;
		}
	}
}