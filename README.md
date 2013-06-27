csharp-distributed-ga
=====================

This project implements a simple distributed genetic algorithm in C# using MongoDB on the backend. Task evaluation can take place in languages other than C#: A C# worker, a Unity worker, and a generalized web worker interface are included. 

The software should work wherever mono/.NET, PHP, and Mongo are supported (e.g. Linux, Windows, and OS X).

Overview
--------

Genetic algorithms often involve significant computation to evaluate new individuals, often because of costly simulations (e.g. you want to evolve a brain for a robot that has many physical parts that must be realistically simulated). 

Luckily, GAs play nice with parallelism (commonly, running a GA consists of many generations of independent evaluations of every individual in the population). Thus sometimes it is nice to have a **distributed** genetic algorithm in which one central server keeps track of the results and many worker computers can grab individual genomes, evaluate them, and then post the results back to the server. In this way, you can achieve a nice speed up in the GA from as many computers as you have available (assuming that the bottleneck is how long each evaluation takes, which is most often the case in real-world problems).

This particular implementation has the distributed GA written in C#, and depends on a Mongo database, which can be hosted locally or on a remote webserver. Worker clients can be written in any language although C# will likely be the most straight-forward. An example C# worker, an example Unity engine worker, and a general web-based worker interface (written in PHP) are provided.
 
Configuration
-------------

The basic C# configuration requires access to a mongo database (by default the c# example connects to localhost, e.g. a local database hosted on the same computer as the C# install). Mongo can be downloaded from http://www.mongodb.org/ and is multi-platform.

Running the Unity example requires a webserver (which can be locally hosted, e.g. through WAMP on windows or a local Apache/nginx install on Linux). The webserver configuration requires PHP (with the PHP MongoDB extension http://www.php.net/manual/en/mongo.installation.php).


Getting Started
---------------

The simplest way to get started is to install a local MongoDB server (download from http://www.mongodb.org/), make sure it is running, and then run the C# dist-ga project, which will populate the database with initial tasks. You can then run one or more instances of the C# simple-consumer executable which will then start evaluating the tasks and storing the results back to the database.


File Structure
-------------------

- **csharp-dist-ga**:
  The C# implementation of the distributed genetic algorithm and an example
  worker C# implementation.

- **simple-unity-client**:
  A Unity game-engine client that can grab tasks from the C# server
  *Note: requires a webserver running the generic php interface below*

- **webserver-php-for-generic-clients**:
  PHP scripts for a webserver, which can enable writing clients for any
  language capable of HTTP form requests. Note that this requires a webserver 
  running PHP with the MongoDB extension.
