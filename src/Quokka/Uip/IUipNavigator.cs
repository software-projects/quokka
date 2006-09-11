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
