csharp-distributed-ga
=====================

Simple distributed genetic algorithm in C# using MongoDB on the backend, with a c# worker, a Unity worker, and a generalized web worker interface. Multi-platform wherever mono/.NET, PHP, and Mongo are supported (e.g. Linux, Windows, OS X).

Overview
--------

Genetic algorithms often involve significant computation to evaluate new individuals, often because of costly simulations (e.g. you want to evolve a brain for a robot that has many physical parts that must be realistically simulated). 

Luckily, GAs play nice with parallelism (each generation of an EA consists *generally* of independent evaluations of each individual in the population). Thus sometimes it is nice to have a **distributed** genetic algorithm in which one central server keeps track of the results and many worker computers can grab individual genomes, evaluate them, and then post the results back to the server. In this way, you can scale the GA to as many computers as you have available, which may significantly speed up the process.

This particular implementation has the main evolutionary server written in C#, and depends on a Mongo database, which can be hosted on a webserver. Worker clients can be written in any language although C# will likely be the most straight-forward. An example C# worker, an example Unity engine worker, and a general web-based worker interface (written in PHP) are provided.
 
Configuration
-------------

The basic C# configuration requires access to a mongo database (by default the c# example connects to localhost, e.g. a local database hosted on the same computer as the C# install). Mongo can be downloaded from http://www.mongodb.org/ and is multi-platform.

Running the Unity example requires a webserver (which can be locally hosted, e.g. through WAMP on windows or a local Apache/nginx install on Linux). The webserver configuration requires PHP (with the PHP MongoDB extension http://www.php.net/manual/en/mongo.installation.php).

File Structure
-------------------

- **csharp-dist-ga**
  The C# implementation of the distributed genetic algorithm and an example
  worker C# implementation.

- **simple-unity-client**
  A Unity game-engine client that can grab tasks from the C# server
  *Note: requires a webserver running the generic php client below*

- **webserver-php-for-generic-clients**
  Through these PHP scripts on the webserver you can write consumers for any
  language that can do HTTP requests.
