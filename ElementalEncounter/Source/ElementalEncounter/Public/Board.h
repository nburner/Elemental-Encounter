// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "string"
#include "Board.generated.h"

using std::string;

typedef uint64_t bitboard;

/*Arranged the squares to look like the board
Below is a picture of the board for my own sake
So A1 is 0, and H8 is 63, and it counts from left to right, bottom to top

56 57 58 59 60 61 62 63
48 49 50 51 52 53 54 55
40 41 42 43 44 45 46 47
32 33 34 35 36 37 38 39
24 25 26 27 28 29 30 31
16 17 18 19 20 21 22 23
8  9  10 11 12 13 14 15
0  1  2  3  4  5  6  7

*/
enum Square : __int8 {
	A8 = 56, B8, C8, D8, E8, F8, G8, H8,
	A7 = 48, B7, C7, D7, E7, F7, G7, H7,
	A6 = 40, B6, C6, D6, E6, F6, G6, H6,
	A5 = 32, B5, C5, D5, E5, F5, G5, H5,
	A4 = 24, B4, C4, D4, E4, F4, G4, H4,
	A3 = 16, B3, C3, D3, E3, F3, G3, H3,
	A2 = 8, B2, C2, D2, E2, F2, G2, H2,
	A1 = 0, B1, C1, D1, E1, F1, G1, H1
};

//a pair of squares is known as a move
//the first square is the from, the second square is the to
typedef std::pair<Square, Square> move;

//Add a direction to a square and you get the square in that direction
//So A1 + NORTH is A2, while B4 + SOUTHEAST is C3
enum Direction : __int8 {
	NORTH = 8,
	EAST = 1,
	SOUTH = -8,
	WEST = -1,

	NORTHEAST = 9,
	SOUTHEAST = -7,
	SOUTHWEST = -9,
	NORTHWEST = 7
};

//A simple enum for black and white
//Useful because you can make an array and use white or black as the subscript
enum Turn : bool { WHITE, BLACK };

UCLASS()
class ELEMENTALENCOUNTER_API ABoard : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	virtual void Tick(float DeltaTime) override;
	~ABoard();
	static ABoard* getInstance();
	bool hasOwnPiece(string area);
	char& operator[](Square i) { return space[i]; }
	char operator[](Square i) const { return space[i]; }
	void updateBoard(::move, Turn);
	Turn justTaken;

	int x;
	int y;

	
protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	ABoard();

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	char space[64];

	ABoard(ABoard&);
	static ABoard* instance;
};
