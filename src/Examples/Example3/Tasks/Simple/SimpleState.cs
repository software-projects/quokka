namespace Example3.Tasks.Simple
{
	public class SimpleState
	{
		private static int _taskCount;
		private readonly int _taskNumber;

		public SimpleState()
		{
			_taskNumber = ++_taskCount;
		}

		public int TaskNumber
		{
			get { return _taskNumber; }
		}
	}
}