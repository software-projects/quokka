using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Services
{
    /// <summary>
    ///		Interface used for generating globally unique identifiers (GUIDs)
    /// </summary>
    /// <remarks>
    ///		Using an instance of this object instead of the <see cref="Guid.NewGuid"/>
    ///		method allows for mock objects during testing.
    /// </remarks>
    public interface IGuidProvider
    {
        /// <summary>
        ///		Generate a new <see cref='Guid'/>
        /// </summary>
        /// <returns>
        ///		Returns a <see cref='Guid'/>.
        /// </returns>
        Guid NewGuid();
    }

    /// <summary>
    /// Guid provider for normal use.
    /// </summary>
    public class GuidProvider : IGuidProvider
    {
        public Guid NewGuid() {
            return Guid.NewGuid();
        }
    }
}
