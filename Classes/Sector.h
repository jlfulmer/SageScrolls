#pragma once

#include "Element.h"

class Sector
{
public:
	Sector();
	~Sector();

	void setElement(Element *);
private:
	Element * element;

};