#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Represents a message that will be displayed to the user of the application via
	/// 	a modal dialog box.
	/// </summary>
	public class UIMessage
	{
		private UIAnswerCollection _possibleAnswers = new UIAnswerCollection();

		/// <summary>
		/// 	Classifies the message as informational, a warning, reporting success or failure,
		/// 	etc.
		/// </summary>
		/// <remarks>
		/// 	The implementation will typically use this property to decide what sort of visual
		/// 	cue to provide to the user.
		/// </remarks>
		public UIMessageType MessageType { get; set; }

		/// <summary>
		/// 	Main instruction of the message. This is usually a one sentence summary of
		/// 	the message.
		/// </summary>
		/// <remarks>
		/// 	The implementation will typically display this message in a more prominent
		/// 	font than the content of the message.
		/// </remarks>
		public string MainInstruction { get; set; }

		/// <summary>
		/// 	A more detailed description of the message. CR-LFs are displayed.
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 	The answer that the user selected in response to the message being displayed.
		/// </summary>
		public UIAnswer SelectedAnswer { get; set; }

		/// <summary>
		/// 	A collection of possible answers that the user can choose from.
		/// </summary>
		/// <remarks>
		/// 	If this collection is left empty, then the user is offered a single response
		/// 	(<see cref = "UIAnswerType.OK" />) to choose from.
		/// </remarks>
		public UIAnswerCollection PossibleAnswers
		{
			get { return _possibleAnswers; }
			set { _possibleAnswers = value ?? new UIAnswerCollection(); }
		}
	}
}