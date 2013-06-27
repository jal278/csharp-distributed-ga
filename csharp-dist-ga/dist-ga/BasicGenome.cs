using System;
using MongoDB.Bson;

namespace distga
{
	public class BasicGenome
	{
		private static Random _rng;
		public static Random getRNG() {
			if (_rng==null)
				_rng=new Random();
			return _rng;
		}
		
		public double gene;
		public double fitness;
	
		public static BsonDocument toTask(BasicGenome g) {
			return new BsonDocument { {"genome",g.gene},{"fitness",g.fitness} };
		}

		public static BasicGenome fromTask(BsonDocument t) {
			BasicGenome g= new BasicGenome();
			g.gene=(double)t["genome"];
			g.fitness=(double)t["fitness"];
			return g;
		}
		
		public BasicGenome ()
		{
			gene=0.0;
			fitness=0.0;
		}
		
		public void mutate()
		{
			Random rng = BasicGenome.getRNG();
			gene+=rng.NextDouble()/1.0;
		}
		
		public BasicGenome clone() {
			BasicGenome clone = new BasicGenome();
			clone.fitness=fitness;
			clone.gene=gene;
			return clone;
		}
	}
}

