// AILibrary.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include <stdio.h>

extern "C"
{
	__declspec(dllexport) void DisplayHelloFromDLL()
	{
		printf("Differenter Hello from DLL !\n");
	}
}
