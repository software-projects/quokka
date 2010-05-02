using System.Data;
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// Implementation of <see cref="ISqlQueryReader{T}"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class QueryReader<T> : ISqlQueryReader<T> where T : class, new()
	{
		private readonly DataRecordConverter _converter;
		private readonly IDataReader _dataReader;
		private readonly T _record;

		public QueryReader(IDataReader dataReader)
		{
			_dataReader = Verify.ArgumentNotNull(dataReader, "dataReader");
			_record = new T();
			_converter = DataRecordConverter.CreateConverterFor(typeof(T), dataReader);
		}

		#region ISqlQueryReader<T> Members

		public void Dispose()
		{
			_dataReader.Dispose();
		}

		public T Record
		{
			get { return _record; }
		}

		public bool Read()
		{
			if (!_dataReader.Read())
			{
				return false;
			}

			_converter.CopyTo(_record);
			return true;
		}

		#endregion
	}
}