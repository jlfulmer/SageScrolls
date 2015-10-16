#pragma once

class Element
{
public:
	enum Type {Fire, Wind, Water, Earth};

	Element(Type);
	~Element();

	const char * getDisplayString();

private:
	// String representations of each Element type
	static const char * TypeStrings[];

	Type type;
};