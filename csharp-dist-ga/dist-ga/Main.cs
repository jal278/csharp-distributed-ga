using System;

namespace distga
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//run simple proof of concept example		
			SimpleGA ga = new SimpleGA();
			ga.doEvolution();
			Console.WriteLine ("Completed");
		}
	}
}
