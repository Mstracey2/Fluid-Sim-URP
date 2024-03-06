

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
    
    
uint HashCoords(float3 position, float size)
{
    position = (uint3) position;
    uint h = (position[0] * 92837111) + (position[1] * 689287499) + (position[2] * 283923481);
    return h % size;
}
    
int3 intCoord(float3 pos, float radius)
{
    return (int3) floor(pos / radius);
}
    
    
