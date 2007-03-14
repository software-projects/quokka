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

namespace Quokka.Services
{
	using System;

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
