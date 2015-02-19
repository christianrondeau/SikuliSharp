namespace SikuliSharp
{
	public interface ISikuli
	{
		bool Exists(IPattern pattern);
		bool Click(IPattern pattern);
	}
}