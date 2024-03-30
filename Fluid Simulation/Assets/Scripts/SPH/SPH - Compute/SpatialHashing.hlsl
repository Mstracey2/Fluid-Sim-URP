

static const int3 offsets3D[27] = //FINALLY, I understand what this is doing. Foreach particle, its not just checking the cell its in, but the neighboring cells too.
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
    uint h = (position[0] * 15823) + (position[1] * 9737333) + (position[2] * 440817757);
    return h;
}

uint HashKey(uint position, uint size)
{
    return position % size;
}
    
int3 intCoord(float3 pos, float radius)
{
    return (int3) floor(pos / radius);
}
    
    
