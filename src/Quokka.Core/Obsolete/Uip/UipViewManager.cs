﻿using System;
using Quokka.Diagnostics;
using Quokka.UI.Messages;
using Quokka.Uip;

// ReSharper disable CheckNamespace
namespace Quokka.Uip
{
	/// <summary>
	/// Implementation of <see cref="IUipViewManager"/> for backwards
	/// compatibility only.
	/// </summary>
	[Obsolete("For backwards compatibility only. Not for new code.")]
	internal class UipViewManager : IUipViewManager
	{
		private readonly UIMessageBox _messageBox;

		public UipViewManager(UIMessageBox messageBox)
		{
			_messageBox = Verify.ArgumentNotNull(messageBox, "messageBox");
		}

		public UipAnswer AskQuestion(UipQuestion question)
		{
			var message = new UIMessage
			{
				Content = question.Content,
				MainInstruction = question.MainInstruction
			};
			switch (question.QuestionType)
			{
				case UipQuestionType.Failure:
					message.MessageType = UIMessageType.Failure;
					break;
				case UipQuestionType.Forbidden:
					message.MessageType = UIMessageType.Forbidden;
					break;
				case UipQuestionType.Warning:
					message.MessageType = UIMessageType.Warning;
					break;
				case UipQuestionType.Question:
					message.MessageType = UIMessageType.Question;
					break;
				case UipQuestionType.Success:
					message.MessageType = UIMessageType.Success;
					break;
				case UipQuestionType.Information:
					message.MessageType = UIMessageType.Information;
					break;
				case UipQuestionType.Unauthorized:
					message.MessageType = UIMessageType.Unauthorized;
					break;
			}
			foreach (var possibleAnswer in question.PossibleAnswers)
			{
				UIAnswer answer;
				switch (possibleAnswer.AnswerType)
				{
					case UipAnswerType.No:
						answer = new UIAnswer(UIAnswerType.No);
						break;
					case UipAnswerType.Ignore:
						answer = new UIAnswer(UIAnswerType.Ignore);
						break;
					case UipAnswerType.Retry:
						answer = new UIAnswer(UIAnswerType.Retry);
						break;
					case UipAnswerType.Abort:
						answer = new UIAnswer(UIAnswerType.Abort);
						break;
					case UipAnswerType.Cancel:
						answer = new UIAnswer(UIAnswerType.Cancel);
						break;
					case UipAnswerType.OK:
						answer = new UIAnswer(UIAnswerType.OK);
						break;
					case UipAnswerType.Yes:
						answer = new UIAnswer(UIAnswerType.Yes);
						break;
					default:
						answer = new UIAnswer(possibleAnswer.ToString());
						break;
				}
				if (possibleAnswer.Callback != null)
				{
					answer.Callback = new CallbackHelper(question, possibleAnswer).Callback;
				}
				message.PossibleAnswers.Add(answer);
			}
			_messageBox.Show(message);
			return question.SelectedAnswer;
		}

		private class CallbackHelper
		{
			private readonly UipAnswer _uipAnswer;
			private readonly UipQuestion _uipQuestion;

			public CallbackHelper(UipQuestion question, UipAnswer uipAnswer)
			{
				_uipAnswer = uipAnswer;
				_uipQuestion = question;
			}

			public void Callback()
			{
				_uipQuestion.SelectedAnswer = _uipAnswer;
				_uipAnswer.Callback();
			}
		}
	}
}
