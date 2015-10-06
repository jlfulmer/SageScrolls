#pragma once

#include "cocos2d.h"

class CustomMenuItem : public cocos2d::Sprite
{
public:
	static CustomMenuItem* createSprite(cocos2d::Point, const std::string&);

	bool init();

	virtual bool onTouchBegan(cocos2d::Touch*, cocos2d::Event*);
	virtual void onTouchEnded(cocos2d::Touch*, cocos2d::Event*);

	CREATE_FUNC(CustomMenuItem);

private:
	Sprite* initSprite(const std::string&);

	void setLabel(const std::string&);
	void setChild(Sprite*, cocos2d::Vec2, bool);
	void performToggle(cocos2d::Touch*);

	Sprite* dimLeft;
	Sprite* dimMiddle;
	Sprite* dimRight;

	Sprite* litLeft;
	Sprite* litMiddle;
	Sprite* litRight;

	bool toggled;
};
