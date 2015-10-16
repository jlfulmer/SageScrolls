#pragma once

class Player
{
public:
	Player();
	~Player();

	int health;
	int score;

private:
	const int MAX_HEALTH = 10;
};