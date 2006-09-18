using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip
{
    /// <summary>
    /// Interface passed to controllers allowing the controller to end the task.
    /// </summary>
    /// <remarks>
    /// Perhaps this should be part of the <c>IUipNavigator</c> interface.
    /// </remarks>
    public interface IUipEndTask
    {
        /// <summary>
        /// End the current task
        /// </summary>
        void EndTask();
    }
}
