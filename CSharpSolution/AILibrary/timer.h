#pragma once
#include <ctime>

class timer
{
	clock_t _start;
	bool started;
public:
	timer();
	~timer();
	void start();
	void reset();
	double read() const;
};

