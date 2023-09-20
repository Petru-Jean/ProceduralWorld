# Procedural World
Side-scroller survival game set in a procedurally generated world.

# Key Concepts 

The game consists of a player that can move freely around a procedurally generated 2d map and modify it by placing or removing blocks. 
The map is split into multiple Chunks and each Chunk consists of multiple blocks. 

A Block is the structure that makes up the Chunks and each Block inherits the IBlock interface and can have unique proprieties.

The surface of the map is generated using layered 3D Perlin Noise, where the third coordinate is the seed of the map.

For generating other elements such as vegetation, ore veins and holes in the terrain, the hashing function xxHash is used.

# Implementation 

The main parts of the Chunk GameObject are as follows:

ChunkData     -> acts as the model component, stores block data for each chunk and allows it to be modified

ChunkRenderer -> creates and updates the visual mesh for each chunk 

Blocks are assigned using the factory object called FlyweightBlock.

For updating the map, the PlayerMapUpdater script calculates wether the needed chunks exist and removes the ones that are too far away.

ChunkCache   - acts as a cache for the Chunk GameObjects

ChunkFactory - instantiates chunk GameObjects based on the provided world position

The map is split into Biomes, based on the world position of the chunk. Each biome contains different types of chunks that are made out of different blocks. IBiome is the abstract class that is implemented by every biome and handles the task of generating the block data for a given chunk in the biome.
