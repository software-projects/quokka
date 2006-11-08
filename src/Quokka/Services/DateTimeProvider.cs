#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
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

        /// <summary>
        ///     Is the date/time returned by this object fake.
        /// </summary>
        /// <returns><c>true</c> if the date/time is fake.</returns>
        /// <remarks>
        ///     This method is available so that production systems can check to ensure
        ///     that they are not using a date/time provider intended for testing.
        /// </remarks>
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
