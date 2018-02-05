#include "AI.h"
using namespace AI;

vector<AIEngine::AI*> AIEngine::_generatedAIs;

AIEngine::AIEngine() {
	
}

AIEngine::~AIEngine() {
	for (int i = 0; i < _generatedAIs.size(); i++) {
		delete _generatedAIs[i];
	}
	_generatedAIs.clear();
}

AIEngine::AI* AIEngine::start(AIType type) {
	AI * result;
	switch (type)
	{
	case AIEngine::B_OFFENSE:
		result = new BasicOffense();
		break;
	case AIEngine::B_DEFENSE:
		result = new BasicDefense();
		break;
	case AIEngine::B_RANDOM:
		result = new BasicRandom();
		break;
	case AIEngine::RANDOM_PET:
		result = new Pet();
		break;
	case AIEngine::DARYLS_PET:
		result = new Pet(0);
		break;
	default:
		break;
	}
	_generatedAIs.push_back(result);
	return result;
}

::move AI::AIEngine::AI::getMove(GameBoard * gp)
{
	return (*this)(*gp);;
}
