#pragma once

#include "Sector.h"
#include "Element.h"
#include "Player.h"

class Game
{
public:
	Game();
	~Game();

	void placeElement(Element *, Sector *);

	void submit();

private:
	Player * player;
	Sector * sectors[4];
	Element * scrolls[12];
};