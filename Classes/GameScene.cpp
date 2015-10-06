#include "GameScene.h"
#include "CustomMenuItem.h"

USING_NS_CC;

Scene* GameScene::createScene() {
	auto scene = Scene::create();
	auto layer = GameScene::create();
	scene->addChild(layer);

	return scene;
}

bool GameScene::init() {
	if (!Layer::init()) {
		return false;
	}

	// initialize and set background image
	auto director = Director::getInstance();
	auto background = Sprite::create("Menus/background.png");
	auto visibleRect = director->getOpenGLView()->getVisibleRect();

	background->setScaleX(visibleRect.size.width / background->getContentSize().width);
	background->setScaleY(visibleRect.size.height / background->getContentSize().height);

	float xPoint = visibleRect.origin.x + visibleRect.size.width / 2;
	float yPoint = visibleRect.origin.y + visibleRect.size.height / 2;
	background->setPosition(Point(xPoint, yPoint));

	// intialize menu buttons
	int xStart = xPoint;
	int yStart = visibleRect.origin.y + visibleRect.size.height / 4;
	auto start = CustomMenuItem::createSprite(Point(xStart, yStart), "end");

	this->addChild(background, -1);
	this->addChild(start, 0);
	return true;
}