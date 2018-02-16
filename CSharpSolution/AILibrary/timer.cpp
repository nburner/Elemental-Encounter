#include "stdafx.h"
#include "timer.h"


timer::timer()
{
}


timer::~timer()
{
}

void timer::start()
{
	_start = std::clock();
}

void timer::reset()
{
	_start = std::clock();
}

double timer::read()
{
	return (std::clock() - _start) / (double)CLOCKS_PER_SEC;;
}
