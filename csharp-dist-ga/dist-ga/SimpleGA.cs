using System;
using System.Collections.Generic;
using System.Threading;

//we use a mongo database as the back-end (www.mongodb.org/‎)
using MongoDB.Bson;
using MongoDB.Driver;

namespace distga
{
	public class SimpleGA
	{
		int populationSize=50;
		int generation;
		int generationCount=20;
		
		//if your db has multiple kinds of tasks in it (perhaps multiple kinds of experiments running)
		//you can differentiate those tasks by taskType
		string taskType="simple_number_task";
		
		//if you want to do multiple runs of experiments it's important that tasks in the db also
		//have a unique run identifier 
		string runId="simple_id";
		
		private List<BasicGenome> population;
		
		//this class takes care of our interaction with the mongo db backend
		database db;
		
		public SimpleGA ()
		{
			generation=0;
			
			db = new database(taskType,runId);
			
			//clobber existing tasks of this taskType for this runId so we don't get confused if
			//a partially-complete run messes up
			db.clearTasks();
			
			//initialize population
			population= new List<BasicGenome>();
			for(int i=0;i<populationSize;i++) {
				population.Add(new BasicGenome());
			}	
		}
		
		//simple fitness function
		public static double fitnessFunction(BasicGenome g) {
			return -Math.Pow(g.gene-10.0,2);
		}
		
		//choose randomly among a given list
		public static T RandomChoice<T> (List<T> source) {
  			Random rnd = new Random();
			int cnt = rnd.Next(source.Count-1);
  			return source[cnt];
		}
		
		//diagnostic method to monitor if fitness is increasing over time
		public double avgFitness() {
			double sum=0.0;
			foreach(BasicGenome g in population) sum+=g.fitness;
			return sum/population.Count;
		}
		
		//simple reproduction loop (no crossover)
		public void reproducePopulation() {
			
			Console.WriteLine("Generation " + generation+ " avg fitness: " + avgFitness());
			
			List<BasicGenome> newPop= new List<BasicGenome>();
			
			for(int i=0;i<populationSize;i++) {
				BasicGenome baby;
				BasicGenome p1 = RandomChoice(population);
				BasicGenome p2 = RandomChoice(population);
			
				if(p1.fitness>p2.fitness)
					baby=p1.clone();
				else
					baby=p2.clone();
				
				baby.mutate();
				newPop.Add(baby);
			}
			
			population=newPop;
		}
		
		public void doEvolution() {
			
			while(generation<generationCount) {		

				//for each genome we need to serialize the genome into something
				//the database can understand (BsonDocument) which is like a hash table
				foreach(BasicGenome g in population) {
					//serialize genome
					BsonDocument task = BasicGenome.toTask(g);
					//send to db
					db.addTask(task,generation);
				}
			
				int tasksRemaining;
				
				//now wait for workers to complete all the tasks
				do { 
					Thread.Sleep(2000); //2 sec wait
					tasksRemaining=db.incompleteTasks(generation);
					Console.WriteLine("Left to do: " + tasksRemaining);
				} while(tasksRemaining>0);
	
				//get result tasks from db
				MongoCursor results = db.getResults(generation,true);
						
				//clear population, then refill with unserialized result genomes
				population.Clear();
				foreach(BsonDocument result in results) {
					population.Add(BasicGenome.fromTask(result));
				}
				
				generation++;
				reproducePopulation();
			}	

		}	
	}
}

