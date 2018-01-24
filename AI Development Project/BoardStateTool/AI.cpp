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
	case AIEngine::PET:
		result = new Pet();
		break;
	default:
		break;
	}
	_generatedAIs.push_back(result);
	return result;
}