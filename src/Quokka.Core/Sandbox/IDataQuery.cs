using System;
using System.Collections.Generic;

namespace Quokka.Sandbox
{
	// This could be implemented by SqlQuery<T>
	public interface IListQuery<T>
	{
		IList<T> ExecuteList();
	}

	public interface ISingleQuery<out T>
	{
		T ExecuteSingle();
	}

	// not a brilliant name
	public interface INonQuery
	{
		int ExecuteNonQuery();
	}
}
