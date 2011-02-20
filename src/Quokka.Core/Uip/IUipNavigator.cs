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

using System;

namespace Quokka.Uip
{
    /// <summary>
    /// Controllers use this interface to initiate transitions in the UIP navigation graph.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each UIP task has an associated navigation graph, which consists of nodes and
    /// transitions. Associated with each node is a view, a controller, and one or more
    /// transitions.
    /// </para>
    /// <para>
    /// Each transition is identified by a <c>navigateValue</c> (which is an arbitrary
    /// name) and a destination node. When a controller determines that it is time to 
    /// transition to a new node, it calls the <see cref="Navigate"/> method of its 
    /// navigator. The UIP manager then changes the current node for the task and 
    /// creates the new view and controller for the new node.
    /// </para>
    /// <para>
    /// In more advanced scenarios, a controller may need to know if a transition is
    /// defined. This information may be passed onto the view in order to enable/disable
    /// certain controls on the form. This is where the <see cref="CanNavigate"/> method
    /// is used.
    /// </para>
    /// <para>
    /// So how does a controller get access to its navigator? The easiest way is to
    /// specify that it needs a navigator in its constuctor:
    /// </para>
    /// <code>
    /// public class MyController {
    ///     private readonly IUipNavigator navigator;
    ///     private readonly MyState state;
    /// 
    ///     public MyController(IUipNavigator navigator, MyState state) {
    ///         this.navigator = navigator.
    ///         this.state = state
    ///     }
    /// 
    ///     // ... remaining controller code goes here
    /// }
    /// </code>
    /// <para>
    /// This technique has the significant advantage that it is easy to provide a mock 
    /// navigator object to the controller during unit testing.
    /// </para>
    /// </remarks>
	//[Obsolete("This will be removed from Quokka in a future release")]
	public interface IUipNavigator
    {
        /// <summary>
        /// Determines whether there is a transition defined in the navigation graph.
        /// </summary>
        /// <param name="navigateValue">
        /// Identifies the transition
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if there is a transition with the specified
        /// <paramref name="navigateValue"/> for the current node, <c>false</c>
        /// otherwise.
        /// </returns>
        bool CanNavigate(string navigateValue);

        /// <summary>
        /// Navigate to the next node in the navigation graph.
        /// </summary>
        /// <param name="navigateValue">
        /// Identify the transition to use in the navigation graph.
        /// </param>
        /// <exception cref="UipUndefinedTransitionException">
        /// Thrown when there is no matching transition in the navigation graph, ie
        /// when <see cref="CanNavigate"/> returns <c>false</c> for the given
        /// <paramref name="navigateValue"/>.
        /// </exception>
        void Navigate(string navigateValue);
    }
}
