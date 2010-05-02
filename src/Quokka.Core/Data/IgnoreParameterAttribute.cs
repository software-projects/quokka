namespace Quokka.Data
{
	public class IgnoreParameterAttribute : ParameterAttribute
	{
		public IgnoreParameterAttribute()
		{
			Ignore = true;
		}
	}
}