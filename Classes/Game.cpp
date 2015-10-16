#include "Game.h"

Game::Game() {
	player = new Player();
}

Game::~Game() {
	player->~Player;
	delete player;
}

void Game::placeElement(Element * element, Sector * sector) {
	sector->setElement(element);
}

void Game::submit() {}
