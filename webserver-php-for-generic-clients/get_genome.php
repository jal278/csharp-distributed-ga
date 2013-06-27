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

$task=$collection->findOne(array('assigned'=>False));

if($task==null) {
 $task=$collection->findOne(array('completed'=>False));
}
else {
 $task["assigned"]=True;
 $collection->save($task);
}


echo json_encode($task);

?>
