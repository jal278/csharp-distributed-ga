using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace distga
{
	/// <summary>
	/// Class For connecting to a database (mongodb) to store and retrieve tasks and results
	/// </summary>
	public class database
	{
		private MongoClient client;
		private MongoServer server;
		private MongoDatabase db;
		private MongoCollection<BsonDocument> tasks;
		
		string taskType="default_task";
		string runId="default_id";
		
		//NOTE: mongo will create new 'databases' automatically if one doesn't exist
		string databaseName="tasks";

		/// <summary>
		/// Initializes the <see cref="distga.database"/> class, which connects to mongodb and grabs and saves tasks to it.
		/// </summary>
		/// <param name='_taskType'>
		/// _task type, identifies what sort of task consumer/producer we are, e.g. could be virtual creature
		///  or biped walking or whatever
		/// </param>
		/// <param name='_runId'>
		/// _A run identifier -- ignored by workers (which will take a task indiscriminately)
		/// </param>
		public database (string _taskType,string _runId)
		{
			taskType=_taskType;
			runId=_runId;
			
			//NOTE: this connects to localhost by default
			client = new MongoClient(); 
			
			//get connection to server
			server = client.GetServer();
			
			//connect to particular database within server
			db = server.GetDatabase(databaseName);
			
			//now get 'collection' within this server
			tasks = db.GetCollection<BsonDocument>(taskType);
		}
		
		/// <summary>
		/// Clears any tasks for this run-id, useful for debugging
		/// </summary>
		public void clearTasks() {
			tasks.Remove(Query.EQ ("runID",runId));
		}
		
		/// <summary>
		/// Adds a new task to the database.
		/// </summary>
		/// <param name='task'>
		/// Task, a BSonDocument that contains all task information (e.g. genome)
		/// </param>
		/// <param name='task_group'>
		/// Task_group.
		/// </param>
		public void addTask(BsonDocument task, int task_group) {
			
			//A new task is unassigned and incomplete
			task.Add ("assigned",false);
			task.Add ("completed",false);
			
			//a task is particular to its run
			task.Add ("runID",runId);
			
			//each task has a taskgroup (which is the generation for this example)
			task.Add ("taskgroup",task_group);
			tasks.Insert(task);
		}
		
		/// <summary>
		/// Claims a task for a worker to complete; will claim an unconsumed task preferentially, but will 
		/// take an incomplete task if none are available; if all tasks are complete it returns null.
		/// </summary>
		/// <returns>
		/// A BsonDocument task that should be completed and returned.
		/// </returns>
		public BsonDocument claimTask() {
			BsonDocument task = tasks.FindOne(Query.EQ ("assigned",false));
			
			//if no unassigned tasks, just take an assigned task that is yet uncompleted
			if(task==null) {
				task=tasks.FindOne(Query.EQ("completed",false));
				if (task==null)
					return null;
				return task;
			}
			//if unassigned tasks, take one
			else {
				task["assigned"]=true;
				tasks.Save(task);  //note that this task is assigned in the database
				return task;
			}
		}
		
		/// <summary>
		/// Save the results of a completed task back to the database.
		/// </summary>
		/// <param name='task'>
		/// The completed task (e.g. with assigned fitness)
		/// </param>
		public void completeTask(BsonDocument task) {
			task["completed"]=true;
			tasks.Save(task);
		}
		
		/// <summary>
		/// Returns a count of incomplete tasks to give an EA an idea of what remains
		/// </summary>
		/// <returns>
		/// The tasks.
		/// </returns>
		/// <param name='task_group'>
		/// Task_group.
		/// </param>
		public int incompleteTasks(int task_group) {
			MongoCursor tasks = getResults(task_group,false);
			return (int)tasks.Count();
		}
		
		public MongoCursor getResults(int task_group,bool complete) {
			return tasks.Find(Query.And(Query.EQ("runID",runId),Query.EQ("completed",complete),
																Query.EQ("taskgroup",task_group)));
		}	
	}
}

