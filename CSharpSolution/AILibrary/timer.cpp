#include "timer.h"

timer::timer()
{
	started = false;
}


timer::~timer()
{
}

void timer::start()
{
	if(!started) _start = std::clock();
	started = true;
}

void timer::stop()
{
	started = false;
}

void timer::reset()
{
	if(started)	_start = std::clock();
}

double timer::read() const
{
	if(started) return (std::clock() - _start) / (double)CLOCKS_PER_SEC;
	else return 0;
}
