// test.cpp : Defines the entry point for the console application.
//

#include <stdio.h>
#include "stdafx.h"
#include "../FastNoise/FastNoise.h"

int main()
{
	
	int count = 0;
	float* result;
	
	int x = 0; 
	while (true) {
		result = GetNoiseBlock(3.0, 4.0, 0.1);
		//printf("before: %f ", result[512 * 512 - 1]);
		
		//printf("after: %f \n", result[0]);
	}

	
	

	


}

