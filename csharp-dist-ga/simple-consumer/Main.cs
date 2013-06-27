using System;
using System.Threading;
using distga;
using MongoDB.Bson;
namespace simpleconsumer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//connect to database
			database db = new database("simple_number_task","");
			
			//keep consuming tasks forever!
			while(true) {
				
				//get task
				BsonDocument task=db.claimTask();
				
				//if there is one to be gotten...
				if(task!=null) {
					Console.WriteLine("Grabbing task.");
					
					//evaluate it's fitness
					task["fitness"]=(SimpleGA.fitnessFunction(BasicGenome.fromTask(task)));
					
					//deliberate pause to showcase multiple consumers acting together
					Thread.Sleep (100);
					
					//show the genome we evaluated
					Console.WriteLine("genome: " + task["genome"]);
					
					//save result to the database
					db.completeTask(task);
					
					//celebrate!
					Console.WriteLine("Task completed.");

				}
				else {  
					//if no tasks available, pause a bit then check again
					Console.WriteLine("No tasks. Sleeping.");
					Thread.Sleep(2000);
				}
			}
		}
	}
}
