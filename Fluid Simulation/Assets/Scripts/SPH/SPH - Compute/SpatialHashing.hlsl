

//Below is are all possible 'directions' of neighboring cells (left or right, up or down, forward or back) , the code goes through each of these to gather data from the neigbors aswell as its own cell (0,0,0)
static const int3 offsets3D[27] =
{
		int3(-1, -1, -1),
			int3(-1, -1, 0),
			int3(-1, -1, 1),
			int3(-1, 0, -1),
			int3(-1, 0, 0),
			int3(-1, 0, 1),
			int3(-1, 1, -1),
			int3(-1, 1, 0),
			int3(-1, 1, 1),
			int3(0, -1, -1),
			int3(0, -1, 0),
			int3(0, -1, 1),
			int3(0, 0, -1),
			int3(0, 0, 0),
			int3(0, 0, 1),
			int3(0, 1, -1),
			int3(0, 1, 0),
			int3(0, 1, 1),
			int3(1, -1, -1),
			int3(1, -1, 0),
			int3(1, -1, 1),
			int3(1, 0, -1),
			int3(1, 0, 0),
			int3(1, 0, 1),
			int3(1, 1, -1),
			int3(1, 1, 0),
			int3(1, 1, 1)
};
    
    
uint Hash(int3 position)
{
    position = (uint3) position;
	
	// * the positons by three prime numbers to produce a hash
    uint h = (position[0] * 15823) + (position[1] * 9737333) + (position[2] * 440817757);
    return h;
}

uint HashKey(uint position, uint size)
{
	//by getting the remainder, particles will have simlarities in key from position and size.
    return position % size;
}
    
int3 intCoord(float3 pos, float radius)
{
	//coord is the rounded up value of the position influenced by the radius of the particle (we want to make sure the hash is dependant on the radius too, not just its position)
    return (int3) floor(pos / radius);
}
    
    
