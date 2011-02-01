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

using System.Data;

namespace Quokka.Data
{
	/// <summary>
	/// SQL command that is not strongly typed.
	/// </summary>
	public class SqlQuery : SqlQueryBase
	{
		public SqlQuery() : this(null) {}

		public SqlQuery(IDbCommand cmd) : base(cmd) {}

		public int ExecuteNonQuery()
		{
			CheckCommand();
			PopulateCommand(Command);
			return CommandExecuteNonQuery(Command);
		}

		public IDataReader ExecuteReader()
		{
			CheckCommand();
			PopulateCommand(Command);
			return CommandExecuteReader(Command);
		}

		/// <summary>
		/// Allows the derived class to customise how the command is executed
		/// </summary>
		/// <param name="cmd">Command populated with command text and parameters</param>
		/// <returns>Number of rows affected by the command</returns>
		protected virtual int CommandExecuteNonQuery(IDbCommand cmd)
		{
			return cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Allows the derived class to customse how the command is executed
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		protected IDataReader CommandExecuteReader(IDbCommand cmd)
		{
			return cmd.ExecuteReader();
		}
	}
}