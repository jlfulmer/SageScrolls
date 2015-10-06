#include "CustomMenuItem.h"
#include "GameScene.h"

USING_NS_CC;

/*
	This class combines textures from the old project to create a single button. This was easier to do
	in Microsoft XNA unfortunately, and it would be wise at some point to try and create single "on/off" textures
	to use Cocos2dx's built in MenuItems.
*/
CustomMenuItem* CustomMenuItem::createSprite(cocos2d::Point position, const std::string& label) {
	auto parent = CustomMenuItem::create();
	parent->setPosition(position);
	parent->setLabel(label);
	return parent;
}

bool CustomMenuItem::init() {
	if (!Sprite::init()) {
		return false;
	}
	//NYI: Need to implement proper scaling support for different resolutions.
	//See http://www.cocos2d-x.org/wiki/Detailed_explanation_of_Cocos2d-x_Multi-resolution_adaptation
	this->setScale(2);

	// initialize button sprites
	dimLeft = initSprite("UI/button-dim-left.png");
	dimMiddle = initSprite("UI/button-dim-middle.png");
	dimRight = initSprite("UI/button-dim-right.png");

	litLeft = initSprite("UI/button-lit-left.png");
	litMiddle = initSprite("UI/button-lit-middle.png");
	litRight = initSprite("UI/button-lit-right.png");

	auto listener = EventListenerTouchOneByOne::create();
	listener->onTouchBegan = CC_CALLBACK_2(CustomMenuItem::onTouchBegan, this);
	listener->onTouchEnded = CC_CALLBACK_2(CustomMenuItem::onTouchEnded, this);
	toggled = false;

	_eventDispatcher->addEventListenerWithSceneGraphPriority(listener, this);
	return true;
}

Sprite* CustomMenuItem::initSprite(const std::string& resource) {
	auto sprite = Sprite::create(resource);
	return sprite;
}

void CustomMenuItem::setLabel(const std::string& label) {
	if (!label.empty()) {
		auto menuLabel = Label::createWithTTF(label, "fonts/LithosPro-Black.ttf", 12);

		auto middleWidth = menuLabel->getBoundingBox().size.width;
		auto middleHeight = dimMiddle->getBoundingBox().size.height;

		auto leftWidth = dimLeft->getBoundingBox().size.width;
		auto rightWidth = dimRight->getBoundingBox().size.width;

		/*
			Calculate the total width of all textures and resize the parent texture box to fit over it. Set Opacity
			to 0 to make sure the parent is not visible but still clickable
		*/
		auto totalWidth = leftWidth + middleWidth + rightWidth;

		setTextureRect(cocos2d::Rect(0, 0, totalWidth, middleHeight));
		setOpacity(GLubyte(0));

		/*
			Setup the rest of the textures, stretch the middle to fit the text and aligh the center of the entire button
			to the parent's coordinates.
		*/
		dimMiddle->setTextureRect(cocos2d::Rect(0,  0, middleWidth, middleHeight));
		litMiddle->setTextureRect(cocos2d::Rect(0, 0, middleWidth, middleHeight));

		auto leftPosition = Vec2(0 - ((leftWidth / 2) + (middleWidth / 2)) + (totalWidth / 2), middleHeight / 2);
		auto rightPosition = Vec2(((rightWidth / 2) + (middleWidth / 2) + (totalWidth / 2)), middleHeight / 2);

		menuLabel->setPosition(totalWidth / 2, middleHeight / 2);
		menuLabel->setColor(Color3B::BLACK);
		
		setChild(dimLeft, leftPosition, true);
		setChild(dimMiddle, Vec2(totalWidth / 2, middleHeight / 2), true);
		setChild(dimRight, rightPosition, true);

		setChild(litLeft, leftPosition, false);
		setChild(litMiddle, Vec2(totalWidth / 2, middleHeight / 2), false);
		setChild(litRight, rightPosition, false);

		addChild(menuLabel);
	}
}

/*
	Used to construct the parts of the menu item button, as assets from the XNA
	version are provided in segments
*/
void CustomMenuItem::setChild(Sprite* child, cocos2d::Vec2 pos, bool visible) {
	child->setPosition(pos);
	child->setVisible(visible);
	addChild(child);
}

void CustomMenuItem::performToggle(Touch* touch) {
	bool value = dimLeft->isVisible();
	dimLeft->setVisible(!value);
	dimMiddle->setVisible(!value);
	dimRight->setVisible(!value);

	litLeft->setVisible(value);
	litMiddle->setVisible(value);
	litRight->setVisible(value);
	toggled = true;
}

/////////// Touch Events ////////////////

bool CustomMenuItem::onTouchBegan(Touch* touch, Event* event) {
	bool ret = false;
	auto point = touch->getLocation();
	auto rect = this->getBoundingBox();

	if (!toggled && rect.containsPoint(point)) {
		performToggle(touch);
	}
	return true;
}

void CustomMenuItem::onTouchEnded(Touch* touch, Event* event) {
	bool ret = false;
	auto point = touch->getLocation();
	auto rect = this->getBoundingBox();

	if (toggled) {
		if (rect.containsPoint(point)) {
			Director::getInstance()->replaceScene(TransitionFade::create(1, GameScene::createScene(), Color3B::BLACK));
		}
		performToggle(touch);
		toggled = false;
	}
}