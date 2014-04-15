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
