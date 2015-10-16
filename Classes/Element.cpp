#include "Element.h"


const char * Element::TypeStrings[] = { "Fire", "Wind", "Water", "Earth" };

Element::Element(Type type) {
	this->type = type;
}

Element::~Element() {}

const char * Element::getDisplayString() {
	return TypeStrings[type];
}