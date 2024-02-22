Patrick Schuur 510154
## Introduction
When thinking about my Procedural Art redo I realized that the Half Life City that I chose didn't appeal to me at all as I barely played Half Life and I didn't care much for the style. I also drew the conclusion that it would be easier to write a research document if the city that I chose was actually interesting to me.

For this reason I chose to recreate Gotham City from the Batman comics. I realized there are a lot of versions of Gotham City so I will take whatever I can find online and combine the things together that I like. It will mostly resemble a Gotham City like in this image from the comics.
![[Gotham_skyline1.webp]]


## City Layout 
The first thing that I wanted to look into was the layout of the city. Luckily for me I could find a nice map of how Gotham City is layed out:![[Gotham_map.webp]]

The first thing that I noted was how Gotham City is made up of 3 large islands with a couple of small islands attached. I decided that I wanted to generate the islands procedurally because I don't want to make a one-on-one replica but a city that has the same vibes.
I started with dividing a big terrain piece in Unity up into smaller squares using binary space partitioning. 
![[Pasted image 20240222012822.png]]

I then realised I could maybe make the islands by branching out into the neighbouring squares. I first made an algorithm to assign the neighbours to the nodes. I then picked a random room and made an island from that through branching through the neighbouring rooms. I also added some editor settings. The green parts are the islands with the red spheres being the center squares of the islands:
![[Pasted image 20240222013228.png]]
![[Pasted image 20240222013242.png]]
