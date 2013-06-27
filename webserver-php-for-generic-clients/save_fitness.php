<?php

$m=new Mongo();
$db = $m->selectDB("tasks");

$collection=NULL;
if($_REQUEST['taskType']!=NULL) {
	$collection = $db->selectCollection($_REQUEST['taskType']);
}
else {
 die("must specify task type");
}

$id=$_REQUEST['id'];
$fitness=$_REQUEST['fitness'];

$task=$collection->findOne(array('_id'=> new MongoId($id)));

if($task!=null) {
 $task["completed"]=True;
 $task["fitness"]=floatval($fitness);
 $collection->save($task);
}

echo json_encode($task);

?>
