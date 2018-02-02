// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Board.h"

/**
 * 
 */
class Player
{
public:
	Turn myColor;
	Player();
	~Player();
	virtual ::move getMove(ABoard * gp = NULL) = NULL;
};


class Human : public Player {
	bool isValidMove(const move) const;
public:
	virtual ::move getMove(ABoard * gp = NULL);
};