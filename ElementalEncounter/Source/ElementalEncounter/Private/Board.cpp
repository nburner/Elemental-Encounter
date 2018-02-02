// Fill out your copyright notice in the Description page of Project Settings.

#include "Board.h"

// Called when the game starts or when spawned
//void ABoard::BeginPlay()
//{
//	Super::BeginPlay();
//	
//}
//
//// Called every frame
//void ABoard::Tick(float DeltaTime)
//{
//	Super::Tick(DeltaTime);
//
//}


ABoard::ABoard()
{
	unsigned char bar = 219;

	for (int i = 0; i < 64; i++) {
		if ((i / 8 + i) % 2 == 0) space[i] = bar;
		else space[i] = ' ';
	}

	for (int i = A1; i <= H2; i++) space[i] = 'W';

	for (int i = A7; i <= H8; i++) space[i] = 'B';

	PrimaryActorTick.bCanEverTick = true;
}

ABoard::~ABoard()
{
	instance = NULL;
}

ABoard* ABoard::getInstance()
{
	if (instance == NULL) {
		//instance = new ABoard();
	}

	return instance;

}

bool ABoard::hasOwnPiece(string space)
{
	return true;
}

void ABoard::updateBoard(move move, Turn t)
{
	space[move.first] = ((move.first / 8 + move.first) % 2 == 0 ? 219 : ' ');
	space[move.second] = (t == 0 ? 'W' : 'B');
	justTaken = t;
}

ABoard* ABoard::instance = NULL;

