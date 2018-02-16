#pragma once
#include <ctime>

class timer
{
	clock_t _start;
public:
	timer();
	~timer();
	void start();
	void reset();
	double read();
};

