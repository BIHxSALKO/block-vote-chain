==========================================================================
Lab 7
Author: Patrick Yukman
MPCS 56600, Summer 2018
==========================================================================

I built, tested, and ran my code directly from Visual Studio. To open the project in Visual Studio, just use the submitted .sln file.

--------------------------------------------------------------------------
Notes:
--------------------------------------------------------------------------

!!!!!!! PLEASE READ THIS !!!!!!!

I also sent an email about this, but here's a reminder:

I think I'm probably the only student in the class who's decided to use C# and Windows to develop my blockchain. Unfortunately, I find myself in a bit of a dilemma: I am unable to create a Windows-based docker image for running and deploying my distributed blockchain. The root issue appears to be that I cannot install Docker for Windows since I do not have Windows 10 Pro, Enterprise, or Education. As a result, I am stuck using Docker Toolbox, which worked fine for the previous lab assignments but which I am finding restricts me to using only Linux images. See https://docs.docker.com/docker-for-windows/install/ and https://forums.docker.com/t/can-i-run-windows-containers-using-docker-toolbox/46923

I sunk a lot of hours into trying to retarget my whole project for .NET Core instead of .NET 4.5 to something compiling and running on a Linux image, but it got very messy, especially with the gRPC dependencies. I was also unwilling to purchase and install an upgraded version of the Windows operating system in order to run Docker for Windows solely for the purpose of this project.

I didn't realize I wouldn't be able to easily containerize up my native development environment at this stage (and had I known about this limitation with Docker Toolbox, I probably would have chosen a different language). So in the interest of completing the fundamental requirements of this lab on time, I've made the following modification to the project:

  - Each node is assigned a unique port at startup (via a command line argument)
  - Nodes keep track of both the ip address and port of each peer

This allowed me to simulate the entire network on my machine without the use of Docker, since I can have multiple instances of my blockchain nodes sitting on localhost at different ports.

Again, please note that I am NOT submitting any Docker tarballs for this lab. Everything is run natively from the command line.

--------------------------------------------------------------------------
Operation:
--------------------------------------------------------------------------

Registrar:
	
	Registrar.exe
	
	Note that the registrar always starts on port 50051.
	
	
NikCoinNode:
	
	NikCoinNode.exe <port>

	Note that each node should be started on a unique port.

--------------------------------------------------------------------------
Example Output:
--------------------------------------------------------------------------

I've provided a set of output files should demonstrate that nodes are handshaking appropriately, mining correctly, and forwarding transactions and blocks to other nodes. Nodes validate blocks as they are received and will reject a block if its published hash does not match the computed hash. All example output can be found in the "ExampleOutput" directory.

Provided outputs:

- Registrar.txt shows the output of the registrar program (quite boring)
- Node1.txt shows the first node's output
- Node2.txt shows the second node's output
- Node3.txt shows the third node's output

Of course, you can see all this output yourself if you run the code.