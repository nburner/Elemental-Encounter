#pragma once
#include"Board.h"
#include <algorithm>
#include <vector>

using std::vector;
using std::string;

class AIEngine{
public:
	AIEngine() { _generatedAIs = vector<AI*>(); };

	enum AIType {
		B_OFFENSE, B_DEFENSE
	};

	class AI
	{
	public:
		
	protected:
		virtual string operator()(const Board) const = 0;	

	private:
		friend static AIEngine;
		int weights[20];
	};


	class BasicOffense : public AI {
		virtual string operator()(const Board b) const {
			auto boards = b.validNextBoards();
			return BoardHelpers::to_string(b.blackTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove);
		}
	};

	class BasicDefense : public AI {
		virtual string operator()(const Board b) const {
			auto boards = b.validNextBoards();
			return BoardHelpers::to_string(b.whiteTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove);
		}
	};


	static AI* start(AIType type) {
		AI * result;
		switch (type)
		{
		case AIEngine::B_OFFENSE:
			result = new BasicOffense();
			break;
		case AIEngine::B_DEFENSE:
			result = new BasicDefense();
			break;
		default:
			break;
		}
		_generatedAIs.push_back(result);
		return result;
	}

	~AIEngine() {
		for (int i = 0; i < _generatedAIs.size(); i++) {
			delete _generatedAIs[i];
		}
		_generatedAIs.clear();
	}

private:
	static vector<AI*> _generatedAIs;
};
