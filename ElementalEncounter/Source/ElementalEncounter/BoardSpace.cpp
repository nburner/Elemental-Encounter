// Fill out your copyright notice in the Description page of Project Settings.

#include "BoardSpace.h"


// Sets default values
ABoardSpace::ABoardSpace()
{
	BoardSpace = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("BlockMesh"));
}

//Called when the game starts or when spawned
void ABoardSpace::BeginPlay()
{
	Super::BeginPlay();
	
}



