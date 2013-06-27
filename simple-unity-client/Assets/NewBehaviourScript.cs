using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

public class NewBehaviourScript : MonoBehaviour {

	private Coroutine downloader;

	// Use this for initialization
	void Start () {
		downloader=StartCoroutine(download());
	}
	
	/// <summary>
	/// Connecting to server to download a simple task (eventually could be a complex genome)
	/// </summary>
	IEnumerator download() {		
	
		//keep consuming tasks ad nauseum

		while (true) {
			
		//query server for an appropriate type of task
		WWWForm getform = new WWWForm();
		getform.AddField("taskType","simple_number_task");
		WWW post = new WWW("http://localhost/get_genome.php",getform);
		yield return post;		
		
		double genome_value=0.0;
		
		if(post.error!=null) {
        	print( "Error downloading: " + post.error );
        	return false;
    	}
		
		//the server returns something to us in JSON which we can interpret
		JSONNode genome = JSON.Parse(post.text);
			
		//if we successfully got a task, read it in and compute the fitness
		if(genome!=null) {
				
			genome_value =genome["genome"].AsDouble;
	    
			//calculate error if we are trying to evolve towards the value 10
			double fitness = (genome_value-10);
			fitness*=fitness;
			fitness = -fitness;
			
			//now post fitness back to server, we can identify the task
			//by its id
			WWWForm saveform= new WWWForm();
			saveform.AddField("fitness",Convert.ToString(fitness));
			saveform.AddField("id",genome["_id"]["$id"]);
			saveform.AddField("taskType","simple_number_task");
			post = new WWW("http://localhost/save_fitness.php",saveform);
			
			yield return post;
			
			if(post.error!=null) {
	        	print( "Error downloading: " + post.error );
	        	return false;
	    	}
			else {
				Debug.Log("Task completed: " + post.text);
			}
		}

			//no need to wait in practice; useful for showcasing multiple running instances
			yield return new WaitForSeconds(0.5f);
		}
		
	}
	
}
