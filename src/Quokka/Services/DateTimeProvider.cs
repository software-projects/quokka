using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Services
{
    /// <summary>
    ///		Interface used for generating the current date and time
    /// </summary>
    /// <remarks>
    ///		Using an instance of this object instead of the static <see cref="DateTime"/>
    ///		methods (eg <c>DateTime.Now</c>, <c>DateTime.UtcNow</c>, <c>DateTime.Today</c>
    ///		method allows for mock objects during unit testing.
    /// </remarks>
    public interface IDateTimeProvider
    {
        /// <summary>
        ///		Gets a <see cref="DateTime"/> that is the current local date and time on
        ///		this computer expressed as the coordinated universal time (UTC).
        /// </summary>
        /// <value>
        ///		A <see cref="DateTime"/> whose value is the current UTC date and time.
        /// </value>
        DateTime UtcNow { get; }

        /// <summary>
        ///		Gets a <see cref="DateTime"/> that is the current local date and time on
        ///		this computer.
        /// </summary>
        /// <value>
        ///		A <see cref="DateTime"/> whose value is the current date and time.
        /// </value>
        DateTime Now { get; }

        /// <summary>
        ///		Gets the current date.
        /// </summary>
        /// <value>
        ///		A <see cref="DateTime"/> set to the date of this instance, with the time
        ///		part set to 00:00:00.
        /// </value>
        DateTime Today { get; }

        bool IsFake();
    }

    /// <summary>
    /// DateTime provider for normal use.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow {
            get { return DateTime.UtcNow; }
        }

        public DateTime Now {
            get { return DateTime.Now; }
        }

        public DateTime Today {
            get { return DateTime.Today; }
        }

        public bool IsFake() {
            return false;
        }
    }
}
