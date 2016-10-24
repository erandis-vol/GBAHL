#include <stdio.h>
#include <stdlib.h>

/* copyright 2016 lost
 * based on code in saptapper
 * finds the m4a song table pointer in gba roms
 */

 /*
 some thumb code that is used by every (afaik) m4a engine to select a song
	push {lr}					b500
	lsl r0, r0, #16				0400
	ldr r2, [start + 0x24]		4a07	offset of song table
	ldr r1, [start + 0x28]		4908
	lsr r0, r0, #13				0b40
	add r0, r0, r1				1840
	ldrh r3, [r0, #4]			8883
	lsl r1, r3, #1				0059
	add r1, r1, r3				18c9
	lsl r1, r1, #2				0089
	add r1, r1, r2				1989
	ldr r2, [r1, #0]			680a
	ldr r1, [r0, #0]			6801
	add r2, r0, #0				1c10
	lsr r0, r0, #28				0f00
	...
 */
unsigned char code[30] = {
	0x00, 0xB5, 0x00, 0x04, 0x07, 0x4A, 0x08, 0x49, 
	0x40, 0x0B, 0x40, 0x18, 0x83, 0x88, 0x59, 0x00, 
	0xC9, 0x18, 0x89, 0x00, 0x89, 0x18, 0x0A, 0x68, 
	0x01, 0x68, 0x10, 0x1C, 0x00, 0xF0
};

/* search 'buffer' for 'search', allowing up to 'm' differences */
int looseCompare(unsigned char* buffer, unsigned char* search, int count, int m)
{
	if (m <= 0) return 0;
	
	int d = 0;
	int i;
	for (i = 0; i < count; i++) {
		if (buffer[i] != search[i]) {
			if (++d >= m) return d;
		}
	}
	
	return d;
}

int findSelectSong(unsigned char* buffer, size_t length)
{
	/* loose search for 'code' */
	int offset;
	for (offset = 0; offset < length; offset += 4) {
		if (looseCompare(&buffer[offset], &code[0], 30, 8) < 8) {
			return offset;
		}
	}
	
	return -1;
}

int findSongTable(char* path)
{
	/* open the ROM file */
	FILE* f = fopen(path, "rb");
	if (!f) return -1;
	
	/* find length of ROM */
	fseek(f, 0, SEEK_END);
	size_t size = ftell(f);
	fseek(f, 0, SEEK_SET);
	
	/* read ROM contents into a buffer */
	unsigned char* buffer = malloc(size);
	if (!buffer) return -1;
	
	fread(buffer, size, 1, f);
	fclose(f);
	
	/* find the select song code */
	int selectSongOffset = findSelectSong(buffer, size);
	if (selectSongOffset == -1) {
		free(buffer);
		return -1;
	}
	
	/* pointer to song table is always 40 bytes beyond start of selectsong code */
	selectSongOffset += 40;
	
	
	if (selectSongOffset >= size) {
		free(buffer);
		return -1;
	} else {
		/* pointer at offset is the start of the songtable */
		free(buffer);
		return selectSongOffset;
	}
}

void usage()
{
	printf("usage:\n\tfind_songtable.exe [rom file]\n");
}

int main(int argc, char* argv[])
{
	if (argc == 2) {
		printf("pointer to songtable at: 0x%x\n", findSongTable(argv[1]));
	}
	else {
		usage();
	}
	return 0;
}