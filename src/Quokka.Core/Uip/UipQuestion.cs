//#region Copyright notice
////
//// Authors: 
////  John Jeffery <john@jeffery.id.au>
////
//// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
//// 
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
//// 
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//#endregion
//
//namespace Quokka.Uip
//{
//	using System;
//	using System.Collections.Generic;
//
//	//[Obsolete("This will be removed from Quokka in a future release")]
//	public enum UipQuestionType
//	{
//		Information,
//		Success,
//		Question,
//		Warning,
//		Forbidden,
//		Unauthorized,
//		Failure,
//	}
//
//	//[Obsolete("This will be removed from Quokka in a future release")]
//	public enum UipAnswerType
//	{
//		// These numbers match the values in System.Windows.Forms.DialogResult
//		Custom = 0,
//		OK = 1,
//		Cancel = 2,
//		Abort = 3,
//		Retry = 4,
//		Ignore = 5,
//		Yes = 6,
//		No = 7,
//	}
//
//	//[Obsolete("This will be removed from Quokka in a future release")]
//	public class UipQuestion
//	{
//		private UipQuestionType _questionType;
//		private string _mainInstruction;
//		private string _content;
//		private readonly List<UipAnswer> _possibleAnswers;
//		private UipAnswer _selectedAnswer;
//
//		public UipQuestion()
//		{
//			_possibleAnswers = new List<UipAnswer>();
//		}
//
//		public UipQuestionType QuestionType
//		{
//			get { return _questionType; }
//			set { _questionType = value; }
//		}
//
//		public string MainInstruction
//		{
//			get { return _mainInstruction; }
//			set { _mainInstruction = value; }
//		}
//
//		public string Content
//		{
//			get { return _content; }
//			set { _content = value; }
//		}
//
//		public UipAnswer SelectedAnswer
//		{
//			get { return _selectedAnswer; }
//			set { _selectedAnswer = value; }
//		}
//
//		public IList<UipAnswer> PossibleAnswers
//		{
//			get { return _possibleAnswers; }
//		}
//
//		public UipAnswer AddAnswer(UipAnswerType answerType)
//		{
//			UipAnswer answer = new UipAnswer(answerType);
//			_possibleAnswers.Add(answer);
//			return answer;
//		}
//
//		public UipAnswer AddAnswer(string text)
//		{
//			UipAnswer answer = new UipAnswer(text);
//			_possibleAnswers.Add(answer);
//			return answer;
//		}
//	}
//
//	//[Obsolete("This will be removed from Quokka in a future release")]
//	public delegate void UipAnswerCallback();
//
//	//[Obsolete("This will be removed from Quokka in a future release")]
//	public class UipAnswer
//	{
//		private readonly string _text;
//		private readonly UipAnswerType _answerType;
//		private UipAnswerCallback _callback;
//
//		public UipAnswer(string text) {
//			_text = text;
//			_answerType = UipAnswerType.Custom;
//		}
//
//		public UipAnswer(UipAnswerType answerType) {
//			if (answerType == UipAnswerType.Custom) {
//				throw new ArgumentException("Must supply text for a custom button");
//			}
//			_text = answerType.ToString();
//			_answerType = answerType;
//		}
//
//		public UipAnswerType AnswerType
//		{
//			get { return _answerType; }
//		}
//
//		public UipAnswerCallback Callback
//		{
//			get { return _callback; }
//			set { _callback = value; }
//		}
//
//		public override string ToString() {
//			return _text;
//		}
//	}
//}
